using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.ServiceModel;
using System.ServiceProcess;
using System.Threading;
using System.Xml.Serialization;
using ZondervanLibrary.Harvester.Communication;
using ZondervanLibrary.Harvester.Core.Exceptions;
using ZondervanLibrary.Harvester.Core.Operations;
using ZondervanLibrary.Harvester.Core.Operations.Counter;
using ZondervanLibrary.Harvester.Core.Operations.Demographics;
using ZondervanLibrary.Harvester.Core.Operations.EZProxy;
using ZondervanLibrary.Harvester.Core.Operations.Statista;
using ZondervanLibrary.Harvester.Core.Operations.Sync;
using ZondervanLibrary.Harvester.Core.Operations.WmsInventory;
using ZondervanLibrary.Harvester.Core.Operations.WmsTransactions;
using ZondervanLibrary.Harvester.Core.Permissions;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Core.Scheduling;
using ZondervanLibrary.Harvester.Service.Configuration;
using ZondervanLibrary.SharedLibrary.Collections;
using OperationContext = ZondervanLibrary.Harvester.Core.Operations.OperationContext;

namespace ZondervanLibrary.Harvester.Service
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public partial class HarvesterService : ServiceBase
    {
        private const string ConfigurationFileName = "Configuration.xml";
        private const string ConfigurationFileNameExample = "Configuration(Example).xml";
        
        private static Tuple<string, string> systemEmail;
        private static string[] userEmails;
        private static string[] developerEmails;
        private static string baseDirectory;
        private static string baseDrive;

        private QueueManager queueManager;
        private DateTime lastRead = DateTime.MinValue;

        private string fullPathToConfig;
        private string fullPathToExampleConfig;

        private Timer scheduleTimer;
        private Thread mainBackgroundThread;

        public ServiceHost serviceHost;
        private IHarvesterClientConnection clientConnection;
        
        private enum QueueManagerState
        {
            FirstTimeOperations,
            ConfigurationChanges,
            RunLatestOperations,
            NoConfigurationFile
        }

#if Production
                static string Configuration = "Production";
#elif Test
                static string Configuration = "Test";
#elif Debug
                static string Configuration = "Debug";
#else
                static string Configuration = "";
#endif


        public HarvesterService()
        {
            InitializeComponent();

            //ServiceName = "Harvester Service";
        }

        #region Service Control

        /// <inheritdoc />
        protected override void OnStart(string[] args)
        {
            try
            {
                WriteEventLog("Started: " + ServiceName, EventLogEntryType.Information);

                baseDrive = Path.GetPathRoot(Environment.CurrentDirectory);

                if (string.IsNullOrWhiteSpace(Configuration))
                    baseDirectory = baseDrive + @"Harvester\";
                else
                    baseDirectory = baseDrive + $@"Harvester ({Configuration})\";

                WriteEventLog("Base Directory is: " + baseDirectory, EventLogEntryType.Information);

                fullPathToExampleConfig = baseDirectory + ConfigurationFileNameExample;
                fullPathToConfig = baseDirectory + ConfigurationFileName;

                if (!Directory.Exists(baseDirectory))
                    Directory.CreateDirectory(baseDirectory, FilePermissions.CreateDirectoryPermissions());

                if (!IsExampleConfigurationFilePresent())
                {
                    using (StreamWriter stream = new StreamWriter(fullPathToExampleConfig))
                    {
                        ConfigurationSettings configFile = CreateExampleConfigurationFile();

                        XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationSettings));

                        serializer.Serialize(stream, configFile);
                    }
                }

                File.SetAccessControl(fullPathToExampleConfig, FilePermissions.CreateFilePermissions());

                File.Delete(baseDirectory + "BaseLog.txt");
                File.Delete(baseDirectory + "QueueManager.txt");
            }
            catch (Exception ex)
            {
                Crash(ex);
            }

            var queueManagerState = IsConfigurationFilePresent() ? QueueManagerState.FirstTimeOperations : QueueManagerState.NoConfigurationFile;

            mainBackgroundThread = new Thread(() => RunQueueManager(queueManagerState)) { Name = $"HarvesterService{Configuration} (Thread)", IsBackground = true };

#if Debug
            Debugger.Launch();
#endif

            serviceHost?.Close();

            // Create a ServiceHost for the HarvesterService type and provide the base address.
            serviceHost = new ServiceHost(this);

            mainBackgroundThread.Start();
        }

        /// <inheritdoc />
        protected override void OnStop()
        {
            scheduleTimer.Dispose();
            queueManager?.StopQueueManager();

            if (serviceHost != null)
            {
                serviceHost.Close();
                serviceHost = null;
            }

            try
            {
                File.AppendAllText(baseDirectory + "Current Status.txt", $"\r\nDate Stopped: {DateTime.Now}  ");
                File.SetAccessControl(baseDirectory + "Current Status.txt", FilePermissions.CreateFilePermissions());
            }
            catch (Exception ex)
            {
                WriteEventLog(ex.ToString(), EventLogEntryType.Error);
            }

            WriteEventLog("Service terminated...", EventLogEntryType.Information);
            Dispose();
        }

        /// <inheritdoc />
        protected override void OnShutdown()
        {
            OnStop();
            base.OnShutdown();
        }

        /// <summary>
        /// Major Exception that the Service cannot recover from. The Service should Log the exception that Crashed it before stopping.
        /// </summary>
        /// <param name="ex">The Exception that caused the Service to Crash</param>
        private static void Crash(Exception ex)
        {
            WriteEventLog(ex.ToString(), EventLogEntryType.Error);
            try
            {
                WriteToFile(baseDirectory + "Current Status.txt", $"Date Crashed: {DateTime.Now}\r\n{ex}");
            }
            catch { /*Nothing to Catch*/ }

            WriteEventLog($"While running The Harvester Service on the Remote Server it Crashed when encountering this error: \r\n{ex}", EventLogEntryType.Error);
            SendEmail($"While running The Harvester Service on the Remote Server it Crashed when encountering this error: \r\n{ex}", new[] { "[Test Email]" });
            throw ex;
        }

        #endregion

        #region Operation Logic

        private void RunQueueManager(QueueManagerState state)
        {
            if (state == QueueManagerState.NoConfigurationFile)
            {
                WriteEventLog("Checking for a Configuration File in 15 minutes at " + DateTime.Now.AddMinutes(15), EventLogEntryType.Information);
                scheduleTimer = new Timer(ScheduledCheckForConfigurationFile, null, TimeSpan.FromMinutes(15), Timeout.InfiniteTimeSpan);
                return;
            }

            WaitForFileAccess(fullPathToConfig, FileAccess.Read, FileShare.ReadWrite);
            using (StreamReader file = new StreamReader(fullPathToConfig))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationSettings));
                try
                {
                    ConfigurationSettings configurationSettings = serializer.Deserialize(file) as ConfigurationSettings;
                    ValidateConfigFileIntegrity(configurationSettings);
                    lastRead = DateTime.Now;

                    if (queueManager != null)
                    {
                        switch (state)
                        {
                            case QueueManagerState.ConfigurationChanges:
                                queueManager.UpdateConfigurationFile(configurationSettings);
                                WriteEventLog("Updated Configuration File in Service", EventLogEntryType.Information);
                                break;
                            case QueueManagerState.FirstTimeOperations:
                                Crash(new InvalidOperationException("QueueManager should not be valid at this point"));
                                break;
                        }
                        queueManager.CheckQueue();
                    }
                    else
                    {
                        queueManager = new QueueManager(configurationSettings, baseDirectory);
                        SubscribeToServiceHostEvents();
                        queueManager.CheckQueue();
                        WriteEventLog("Ran Operations for the first time", EventLogEntryType.Information);
                    }
                }
                catch (Exception ex)
                {
                    Crash(ex);
                }
            }

            ScheduleNextRun();
        }

        private void SubscribeToServiceHostEvents()
        {
            if (serviceHost != null)
            {
                queueManager.CollectionPropertyChanged += QueueManagerCollectionPropertyChanged;

                serviceHost.Closed += (sender, args) =>
                {
                    queueManager.PropertyChanged -= QueueManagerOnPropertyChanged;
                    queueManager.CollectionPropertyChanged = null;
                };

                serviceHost.Opened += (sender, args) =>
                {
                    queueManager.PropertyChanged += QueueManagerOnPropertyChanged;
                    queueManager.CollectionPropertyChanged += QueueManagerCollectionPropertyChanged;
                };

                // Open the ServiceHostBase to create listeners and start listening for messages.
                serviceHost.Open();
            }
        }

        private void QueueManagerOnCollectionPropertyChanged(object o, EventArgs eventArgs)
        {
            throw new NotImplementedException();
        }

        private void QueueManagerOnPropertyChanged(object sender, PropertyChangedEventArgs propertyChangedEventArgs)
        {
            throw new NotImplementedException();
        }

        private void QueueManagerCollectionPropertyChanged(PropertyChangedEventArgs p, NotifyCollectionChangedEventArgs e)
        {
            switch (p.PropertyName)
            {
                case "runningOperations":
                    clientConnection.OnRunningOperationsCollectionChanged(new SerializableNotifyCollectionChangedEventArgs(e));
                    break;
                case "canceledOperations":
                    clientConnection.OnCancelledOperationsCollectionChanged(new SerializableNotifyCollectionChangedEventArgs(e));
                    break;
            }
        }

        private void ScheduleNextRun()
        {
            try
            {
                if (queueManager == null)
                {
                    ////Schedule Timer for 15 minutes from now
                    WriteEventLog("No RunDate found. Scheduled for " + DateTime.Now.AddMinutes(15), EventLogEntryType.Warning);
                    scheduleTimer = new Timer(ScheduledUpdate, null, TimeSpan.FromMinutes(15).Milliseconds, Timeout.Infinite);
                }
                else
                {
                    TimeSpan nextRunDate = queueManager.NextRunDate - DateTime.Now;
                    double hourInMilliseconds = TimeSpan.FromHours(1).TotalMilliseconds;

                    ////Schedule to Poll for Configuration File changes in an hour
                    if (nextRunDate.TotalMilliseconds > hourInMilliseconds)
                    {
                        WriteEventLog("Checking for Configuration File Changes in 1 hour at " + DateTime.Now.AddHours(1), EventLogEntryType.Information);
                        scheduleTimer = new Timer(ScheduledUpdate, null, (long)hourInMilliseconds, Timeout.Infinite);
                    }
                    else ////Schedule to Run an operation that is earlier than an hour from now.
                    {
                        WriteEventLog("Running next Operation at " + queueManager.NextRunDate, EventLogEntryType.Information);
                        scheduleTimer = new Timer(ScheduledUpdate, null, (long)nextRunDate.TotalMilliseconds, Timeout.Infinite);
                    }
                }
            }
            catch (Exception ex)
            {
                Crash(ex);
            }
        }

        private void ScheduledUpdate(object sender)
        {
            try
            {
                if (!IsConfigurationFilePresent())
                    throw new FileNotFoundException("Configuration file missing that was previously available");

                if (ConfigurationFileModified())
                {
                    WriteEventLog($"Configuration File was modified at {File.GetLastWriteTime(fullPathToConfig)} since previous modification {lastRead}: ", EventLogEntryType.Information);
                    RunQueueManager(QueueManagerState.ConfigurationChanges);
                }
                else if (HasRunnableOperation())
                {
                    WriteEventLog("Running Scheduled Operation with Run Date: " + queueManager.NextRunDate, EventLogEntryType.Information);
                    RunQueueManager(QueueManagerState.RunLatestOperations);
                }
                else
                {
                    ScheduleNextRun();
                }
            }
            catch (Exception ex)
            {
                WriteEventLog(ex.ToString(), EventLogEntryType.Error);
                if (ex.GetType() != typeof(IOException))
                {
                    Crash(ex);
                }
            }

            queueManager.WriteCurrentStatus();
        }

        private void ScheduledCheckForConfigurationFile(object sender)
        {
            RunQueueManager(IsConfigurationFilePresent() ? QueueManagerState.FirstTimeOperations : QueueManagerState.NoConfigurationFile);
        }

        private bool ConfigurationFileModified()
        {
            return lastRead < File.GetLastWriteTime(fullPathToConfig) || lastRead < File.GetCreationTime(fullPathToConfig);
        }

        private bool HasRunnableOperation()
        {
            return DateTime.Now >= queueManager.NextRunDate;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Check for a Configuration File that we will either use or replace.
        /// </summary>
        /// <param name="firstTime">Is this the First time the Harvester has checked for a Configuration File?</param>
        private bool IsConfigurationFilePresent()
        {
            return File.Exists(fullPathToConfig);
        }

        private bool IsExampleConfigurationFilePresent()
        {
            return File.Exists(fullPathToExampleConfig);
        }

        public static void WriteEventLog(string message, EventLogEntryType type)
        {
            string eventLogSource = string.IsNullOrWhiteSpace(Configuration) ? "Harvester Service" : $"Harvester Service ({Configuration})";
            if (!EventLog.SourceExists(eventLogSource, "."))
                EventLog.CreateEventSource(eventLogSource, "Application");
            EventLog eventLog = new EventLog("Application", ".", eventLogSource);

            eventLog.WriteEntry(message, type);
        }

        private static void WaitForFileAccess(string fullPath, FileAccess access, FileShare share)
        {
            for (int numTries = 0; numTries < 10; numTries++)
            {
                try
                {
                    FileStream fs = new FileStream(fullPath, FileMode.Open, access, share);

                    fs.ReadByte();
                    fs.Seek(0, SeekOrigin.Begin);
                    return;
                }
                catch (IOException)
                {
                    Thread.Sleep(100);
                }
            }

            Crash(new ConfigurationFileException("The Configuration file cannot be actively modified during the harvesting Process"));
        }

        public static void WriteToFile(string filePath, string contents)
        {
            File.WriteAllText(filePath, contents);
            File.SetAccessControl(filePath, FilePermissions.CreateFilePermissions());
        }

        #endregion

        #region Configuration File

        /// <summary>
        /// Create a Configuration File object
        /// </summary>
        /// <returns>A Configuration File Object</returns>
        public static ConfigurationSettings CreateExampleConfigurationFile()
        {
            return new ConfigurationSettings
            {
                Notification = new NotificationSettings
                {
                    SystemEmailAccount = new EmailAccount
                    {
                        UserName = "[Insert Username Here]",
                        Password = "[Insert Password Here]"
                    },
                    DeveloperEmails = new[]
                    {
                        "[Insert Email Here]"
                    },
                    UserEmails = new[]
                    {
                        "[Insert Email Here]"
                    }
                },
                Repositories = new RepositoryArgumentsBase[]
                {
                    new SqlServerDatabaseRepositoryArguments
                    {
                        Name = "Statistics",
                        Server = "[Insert Databas Server]",
                        Database = "Statistics",
                        Authentication = SqlServerAuthenticationMethod.Windows
                    },
                    new SqlServerDatabaseRepositoryArguments
                    {
                        Name = "Harvester",
                        Server = "[Insert Database Server]",
                        Database = "Harvester",
                        Authentication = SqlServerAuthenticationMethod.UsernamePassword,
                        Username = "[Insert Username Here]",
                        Password = "[Insert Password Here]"
                    },
                    new FolderDirectoryRepositoryArguments
                    {
                        Name = "[Name]",
                        Path = @"[C:\Directory\]"
                    },
                    new FtpDirectoryRepositoryArguments
                    {
                        Name = "[Name]",
                        Host = "[FtpHost]",
                        Port = 21,
                        Username = "[Username]",
                        Password = "[Password]",
                        UseSsl = false
                    },
                    new SftpDirectoryRepositoryArguments
                    {
                        Name = "[Name]",
                        Host = "[SftpHost]",
                        Port = 22,
                        Username = "[Username]",
                        Password = "[Password]",
                        DirectoryPath = "[false or true]"
                    },
                    new SushiCounterRepositoryArguments
                    {
                        Name = "[VendorName]",
                        Url = "[http://]",
                        RequestorID = "[RequestorID]",
                        CustomerID = "[CustomerID]",
                        ReleaseVersion = ReleaseVersion.R4,
                        AvailableReports = new[] { CounterReport.JR1, CounterReport.PR1 },
                        JsonRepository = new FolderDirectoryRepositoryArguments
                        {
                            Name = "[Json Local Copy]",
                            Path = @"[C:\JsonLocal\]"
                        },
                    },
                    new EbscoHostCounterRepositoryArguments
                    {
                        Name = "EbscoHost",
                        Username = "[Username]",
                        Password = "[Password]",
                        Sushi = new SushiCounterRepositoryArguments
                        {
                            Url = "[http://]",
                            RequestorID = "[RequestorID]",
                            CustomerID = "[CustomerID]",
                            ReleaseVersion = ReleaseVersion.R4,
                            AvailableReports = new[] { CounterReport.DB1, CounterReport.JR1, CounterReport.JR2, CounterReport.JR3, CounterReport.BR1, CounterReport.BR3 },
                            PlatformCorrections = new [] { new PlatformCorrection { PlatformtoReplace = "[BadPlatform Name]", PlatformReplacingIt = "[ReplacementPlatformName]" }}
                        }
                    },
                },
                Operations = new OperationArgumentsBase[]
                {
                    new ImportCounterTransactionsOperationArguments
                    {
                        Name = "[Vendor Import]",
                        SourceCounter = "[Counter Repository]",
                        DestinationDatabase = "Statistics",
                        HarvesterDatabase = "Harvester",
                        Schedules = new ScheduleArgumentsBase[]
                        {
                            new IntervalScheduleArguments
                            {
                                Interval = 1,
                                Unit = IntervalUnit.Months,
                                StartDate = new DateTime(2014, 1, 1),
                                EndDate = new DateTime(2017, 12, 1)
                            }
                        },
                        MaximumRunsPerDay = 15,
                        MaximumConcurrentlyRunning = 1,
                        LocalJsonStorage = "Json Local Copy"
                    },
                    new ImportDemographicsOperationArguments
                    {
                        Name = "[Demographic Import]",
                        SourceDirectory = "[Demographics Repository]",
                        DestinationDatabase = "Statistics",
                        HarvesterDatabase = "Harvester",
                        Schedules = new ScheduleArgumentsBase[]
                        {
                            new IntervalScheduleArguments
                            {
                                Interval = 8,
                                Unit = IntervalUnit.Weeks,
                                StartDate = DateTime.Now
                            }
                        }
                    },
                    new ImportWmsInventoryOperationArguments
                    {
                        Name = "WMS Inventory Import",
                        SourceDirectory = "[Repository]",
                        DestinationDatabase = "Statistics",
                        HarvesterDatabase = "Harvester",
                        Schedules = new ScheduleArgumentsBase[]
                        {
                            new IntervalScheduleArguments
                            {
                                Interval = 1,
                                Unit = IntervalUnit.Weeks,
                                StartDate = DateTime.Now
                            }
                        }
                    },
                    new SyncOperationArguments
                    {
                        Name = "[Sync Data]",
                        SourceDirectory = "[Repository]",
                        DestinationDirectory = "[Repsitory]",
                        Schedules = new ScheduleArgumentsBase[]
                        {
                            new IntervalScheduleArguments
                            {
                                Interval = 1,
                                Unit = IntervalUnit.Weeks,
                                StartDate = DateTime.Now
                            }
                        },
                        FilePattern = @"[^ITU[.]Item_inventories[.]\d{8}[.]txt$]"
                    },
                    new ImportWmsTransactionOperationArguments
                    {
                        Name = "WMS Transaction Import",
                        SourceDirectory = "[WMSTransaction]",
                        DestinationDatabase = "Statistics",
                        HarvesterDatabase = "Harvester",
                        Schedules = new ScheduleArgumentsBase[]
                        {
                            new IntervalScheduleArguments
                            {
                                Interval = 1,
                                Unit = IntervalUnit.Days,
                                StartDate = DateTime.Now
                            }
                        }
                    },
                }
            };
        }

        /// <summary>
        /// Testing to Validate the Configuration File as Valid
        /// </summary>
        /// <param name="settings">The Configuration File Object</param>
        private void ValidateConfigFileIntegrity(ConfigurationSettings settings)
        {
            Contract.Requires(settings != null);

            // Get email information out of settings first so we can notify if there is a 
            // problem with any of the other settings.
            systemEmail = settings.Notification.SystemEmailAccount.EmailTuple;

            if (systemEmail?.Item1 == null || systemEmail.Item2 == null)
            {
                RaiseError("The system email must be specified.");
            }

            userEmails = settings.Notification.UserEmails;

            if (userEmails == null || userEmails.Length == 0)
            {
                RaiseError("There must be at least one user email specified.");
            }

            developerEmails = settings.Notification.DeveloperEmails;

            if (developerEmails == null || developerEmails.Length == 0)
            {
                RaiseError("There must be at least one developer email specified.");
            }

            if (settings.Repositories == null || settings.Repositories.Length == 0)
            {
                RaiseError("There must be at least one repository specified.");
            }

            try
            {
                Dictionary<string, RepositoryArgumentsBase> repositories = settings.Repositories.ToDictionary(r => r.Name);
                foreach (KeyValuePair<string, RepositoryArgumentsBase> repository in repositories.Where(x => x.Value.GetType() == typeof(SftpDirectoryRepositoryArguments)))
                {
                    SftpDirectoryRepositoryArguments sftpArgs = (SftpDirectoryRepositoryArguments)repository.Value;
                    if (sftpArgs.DirectoryPath.Contains("../"))
                        RaiseError($"DirectoryPath {sftpArgs.DirectoryPath} in repository {sftpArgs.Name} must not contain '../'");
                }
            }
            catch (ArgumentNullException)
            {
                RaiseError("Every resository must have a name.");
            }
            catch (ArgumentException)
            {
                RaiseError("Every repository name must be unique.");
            }

            OperationArgumentsBase[] operations = settings.Operations;

            if (operations == null)
            {
                RaiseError("Every Operation must be valid");
            }
        }

        /// <summary>
        /// An Error in an invalid Configuration File that will be emailed to Users or Developers.
        /// </summary>
        /// <param name="message">The Message to be emailed to User Emails</param>
        private void RaiseError(string message)
        {
            WriteEventLog("Configuration file contains error: " + message, EventLogEntryType.Error);
            try
            {
                if (userEmails != null)
                    SendEmail(message, userEmails);
            }
            catch (Exception ex)
            {
                WriteEventLog(ex.Message, EventLogEntryType.Error);
            }

            Crash(new ConfigurationFileException(message));
        }

        /// <summary>
        /// Functionality to Send an Email to alert Users/Developers of Configuration File Errors
        /// </summary>
        /// <param name="message"></param>
        /// <param name="emailAddress"></param>
        public static void SendEmail(string message, string[] emailAddress)
        {
            MailMessage mailMessage = new MailMessage
            { 
                Subject = "Harvester Service Email Notification",
                Body = $"{message}\n\nHarvester Service {Configuration}", /*mailMessage.From = new MailAddress(string.Format("{0}@taylor.edu",_systemEmail.Item1));*/
                From = new MailAddress("[Test Email]" )
            };
            foreach (string address in emailAddress)
            {
                mailMessage.To.Add(address);
            }

            SmtpClient client = new SmtpClient
            {
                Port = 587,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Host = "smtp.mail.yahoo.com",
                UseDefaultCredentials = false,
                EnableSsl = true,
                ////Credentials = new NetworkCredential(_systemEmail.Item1, _systemEmail.Item2) for Production
                Credentials = new NetworkCredential("[Test Email]", "gmmmawmgjmsnpsaa")
            };
            client.Send(mailMessage);
        }
        #endregion
    }

    public partial class HarvesterService : IHarvesterServiceConnection
    {
        public void Subscribe()
        {
            clientConnection = System.ServiceModel.OperationContext.Current.GetCallbackChannel<IHarvesterClientConnection>();
        }

        public IEnumerable<OperationContext> RunningOperations()
        {
            return queueManager.RunningOperations;
        }
    }

    public partial class HarvesterService
    {
        //public int Running()
        //{
        //    return queueManager.Running;
        //}

        public int CanceledOperations()
        {
            return queueManager.Canceled;
        }

        public int FailedOperations()
        {
            return queueManager.Failures;
        }

        public int SuccessfulOperations()
        {
            return queueManager.Successes;
        }

        public int RetriedOperations()
        {
            return queueManager.Retries;
        }

        public ConfigurationSettings ConfigurationSettings()
        {
            return queueManager.Settings;
        }
    }

    [ServiceContract]
    public interface IHarvester
    {
        [OperationContract]
        int RunningOperations();

        [OperationContract]
        int CanceledOperations();

        [OperationContract]
        int FailedOperations();

        [OperationContract]
        int SuccessfulOperations();

        [OperationContract]
        int RetriedOperations();

        [OperationContract]
        ConfigurationSettings ConfigurationSettings();
    }
}