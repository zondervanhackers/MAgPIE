using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ZondervanLibrary.Harvester.Core.Exceptions;
using ZondervanLibrary.Harvester.Core.Operations;
using ZondervanLibrary.Harvester.Core.Permissions;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Core.Scheduling;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.Harvester.Service.Configuration;
using ZondervanLibrary.Harvester.Service.Properties;
using ZondervanLibrary.SharedLibrary.Collections;

namespace ZondervanLibrary.Harvester.Service
{
    public class QueueManager : INotifyPropertyChanged
    {
        private readonly HeapPriorityQueue<OperationContext> queuedOperations;
        private int skippedOperations;
        private readonly Dictionary<string, CancellationTokenSource> cancellationTokens;
        private readonly Dictionary<string, RepositoryArgumentsBase> repositories;

        private int requestsPostponedForADay;
        private int requestsDelayedForConcurrency;
        private int requestsDelayedForMaxRunning;
        private readonly int maxRunningOperations = 30;
        private readonly int minutesToDelayForConcurrency = 5;
        private readonly int minutesToDelayForMaxRunning = 10;
        private readonly string baseDirectory;
        private readonly Dictionary<string, Dictionary<int, int>> datedOperations; //[mm-dd-yy][OperationId]

        public DateTime NextRunDate = DateTime.MaxValue;

        public int TotalOperationsRun;
        public int Running
        { 
            get
            {
                lock (RunningOperations)
                {
                    return (int) RunningOperations?.Count;
                }
            }
        }
        public int Canceled => CanceledOperations?.Count ?? 0;
        public int Failures => FailedOperations?.Count ?? 0;
        public int Successes => SuccessfulOperations?.Count ?? 0;
        public int Retries => RetriedOperations?.Count ?? 0;

        public ObservableCollection<OperationContext> RunningOperations { get; }
        public ObservableCollection<OperationContext> CanceledOperations { get; }
        public ObservableCollection<OperationContext> FailedOperations { get; }
        public ObservableCollection<OperationContext> SuccessfulOperations { get; }
        public ObservableCollection<OperationContext> RetriedOperations { get; }

        public ConfigurationSettings Settings { get; }

        #region Initialization & Update

        /// <summary>
        /// Manages, Tracks, and Logs all Operations
        /// </summary>
        /// <param name="settings">The Configuration File to be used</param>
        /// <param name="basedirectory">The Full Path to the base directory the Service is going to use</param>
        public QueueManager(ConfigurationSettings settings, string basedirectory)
        {
            queuedOperations = new HeapPriorityQueue<OperationContext>();
            repositories = settings.Repositories.ToDictionary(x => x.Name);
            OperationContextFactory.SetRepositories(repositories);

            RunningOperations = new ObservableCollection<OperationContext>();
            CanceledOperations = new ObservableCollection<OperationContext>();
            FailedOperations = new ObservableCollection<OperationContext>();
            SuccessfulOperations = new ObservableCollection<OperationContext>();
            RetriedOperations = new ObservableCollection<OperationContext>();

            RunningOperations.CollectionChanged += (sender, args) => { OnCollectionChanged(new PropertyChangedEventArgs(nameof(RunningOperations)), args); };
            CanceledOperations.CollectionChanged += (sender, args) => { OnCollectionChanged(new PropertyChangedEventArgs(nameof(CanceledOperations)), args); };
            FailedOperations.CollectionChanged += (sender, args) => { OnCollectionChanged(new PropertyChangedEventArgs(nameof(FailedOperations)), args); };
            SuccessfulOperations.CollectionChanged += (sender, args) => { OnCollectionChanged(new PropertyChangedEventArgs(nameof(SuccessfulOperations)), args); };
            RetriedOperations.CollectionChanged += (sender, args) => { OnCollectionChanged(new PropertyChangedEventArgs(nameof(RetriedOperations)), args); };

            cancellationTokens = new Dictionary<string, CancellationTokenSource>();
            TotalOperationsRun = 0;
            requestsPostponedForADay = 0;
            requestsDelayedForConcurrency = 0;
            datedOperations = new Dictionary<string, Dictionary<int, int>>();
            baseDirectory = basedirectory;
            Settings = settings;

            if (!Directory.Exists(baseDirectory + @"Logs\"))
                Directory.CreateDirectory(baseDirectory + @"Logs\", FilePermissions.CreateDirectoryPermissions());

            WriteCurrentStatus();
            UpdateConfigurationFile(settings);
            string harvesterStartFileName = $@"{basedirectory}Logs\Harvester Start - {DateTime.Now:yyyy-MM-dd}.txt";
            File.WriteAllText(harvesterStartFileName, "Total Operations: " + queuedOperations.Count());
            File.SetAccessControl(harvesterStartFileName, FilePermissions.CreateFilePermissions());
        }

        private void CreateFoldersInRepositories()
        {
            //Create Repositories from Configuration File Settings
            foreach (string key in repositories.Keys)
            {
                if (repositories[key].GetType() == typeof(FolderDirectoryRepositoryArguments))
                {
                    FolderDirectoryRepositoryArguments repository = (FolderDirectoryRepositoryArguments)repositories[key];
                    if (!Directory.Exists(repository.Path))
                        Directory.CreateDirectory(repository.Path, FilePermissions.CreateDirectoryPermissions());
                }
            }
        }

        public void UpdateConfigurationFile(ConfigurationSettings configsettings)
        {
            CreateFoldersInRepositories();
            
            foreach (RepositoryArgumentsBase repository in configsettings.Repositories)
            {
                repositories[repository.Name] = repository;
            }

            OperationContextFactory.SetRepositories(repositories);

            using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(repositories["Harvester"]))
            {
                foreach (KeyValuePair<String, RepositoryArgumentsBase> repo in repositories)
                {
                    if (harvester.DataContext.Repositories.All(x => x.Name != repo.Key))
                    {
                        harvester.DataContext.Repositories.InsertOnSubmit(new Entities.Repository
                        {
                            Name = repo.Key
                        });
                    }
                }

                harvester.DataContext.SubmitChanges();
            }

            foreach (OperationArgumentsBase operationArgs in configsettings.Operations)
            {
                RemoveOldOperations(operationArgs);

                ScheduleArgumentsBase[] schedules = operationArgs.Schedules;

                foreach (ScheduleArgumentsBase schedule in schedules)
                {
                    IntervalSchedule intervalSchedule = new IntervalSchedule((IntervalScheduleArguments)schedule);

                    IEnumerator<DateTime> enumerator = intervalSchedule.GetEnumerator();
                    enumerator.MoveNext();
                    try
                    {
                        AddOperation(OperationContextFactory.CreateOperationContext(operationArgs, enumerator));
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(KeyNotFoundException))
                        {
                            throw new KeyNotFoundException("Repository Missing: " + (KeyNotFoundException)ex.InnerException);
                        }
                        throw;
                    }
                }
            }
        }

        private void RemoveOldOperations(OperationArgumentsBase operationArgs)
        {
            if (Settings.Operations.All(x => x.Name != operationArgs.Name)) return;

            OperationArgumentsBase operationSetting = Settings.Operations.FirstOrDefault(x => x.Name == operationArgs.Name);

            if (operationSetting == null) return;

            if (operationSetting.MaximumRunsPerDay != operationArgs.MaximumRunsPerDay || operationSetting.Schedules.Length != operationArgs.Schedules.Length)
            {
                RemoveOperation(queuedOperations.FirstOrDefault(x => x.Operation.Name == operationArgs.Name));
            }
            else
            {
                for (int i = 0; i < operationArgs.Schedules.Length; i++)
                {
                    if (CompareOperationArguments(operationArgs, operationSetting))
                        continue;

                    RemoveOperation(OperationContextFactory.CreateOperationContext(operationSetting, new IntervalSchedule((IntervalScheduleArguments) operationSetting.Schedules[i]).GetEnumerator()));
                    return;
                }
            }
        }

        private static bool CompareOperationArguments(OperationArgumentsBase operationArgs, OperationArgumentsBase operationSetting)
        {
            return operationArgs.GetType() == operationSetting.GetType() && operationArgs.Equals(operationSetting);
        }

        #endregion

        #region Operation Management

        private void AddOperation(OperationContext operationContext)
        {
            lock (queuedOperations)
                if (RunningOperations != null)
                {
                    lock (RunningOperations)
                    {
                        if (queuedOperations.Contains(operationContext))
                        {
                            throw new ArgumentOutOfRangeException(nameof(operationContext), "This operation has already been queued to run.");
                        }

                        if (RunningOperations.Contains(operationContext))
                        {
                            throw new ArgumentOutOfRangeException(nameof(operationContext), "This operation is already running.");
                        }
                    }
                }

            queuedOperations.Enqueue(operationContext);
        }

        private void RemoveOperation(OperationContext operationContext)
        {
            lock (queuedOperations)
            {
                // Lock to prevent operation from being run while trying to remove it
                if (!queuedOperations.Contains(operationContext))
                {
                    throw new ArgumentOutOfRangeException(nameof(operationContext));
                }

                queuedOperations.Dequeue();
            }
        }

        public void CancelOperation(OperationContext operationContext)
        {
            CancellationTokenSource tokenSource = cancellationTokens.Where(x => x.Key == operationContext.Operation.Name + operationContext.RunDate.ToShortDateString()).Select(x => x.Value).FirstOrDefault();
            if (tokenSource != null && tokenSource.Token.CanBeCanceled)
            {
                tokenSource.Cancel();
            }
            else
            {
                throw new InvalidOperationException("Cancellation Token is null or not found");
            }
        }

        /// <summary>
        /// Checks for runnable Operations in Queue
        /// </summary>
        public void CheckQueue()
        {
            DateTime currentInstant = DateTime.Now;

            List<OperationContext> runnableOperations = new List<OperationContext>();

            lock (queuedOperations)
            {
                while (queuedOperations.Count > 0 && queuedOperations.Peek().RunDate < currentInstant)
                {
                    runnableOperations.Add(queuedOperations.Dequeue());
                }
            }

            foreach (OperationContext operation in runnableOperations)
            {
                RunOperation(operation);
            }

            lock (queuedOperations)
            {
                foreach (OperationContext operation in runnableOperations)
                {
                    OperationContext x = operation.GetNext();

                    if (x != null)
                    {
                        queuedOperations.Enqueue(x);
                    }
                }

                if (queuedOperations.Count > 0)
                {
                    if (queuedOperations.Peek().RunDate < currentInstant)
                        CheckQueue();
                    else
                        NextRunDate = queuedOperations.Min(x => x.RunDate);
                }
            }
        }

        private async void RunOperation(OperationContext operationContext)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            cancellationTokens.Add(operationContext.Operation.Name + operationContext.RunDate.ToShortDateString(), source);

            DateTime startTime = DateTime.Now;

            StringBuilder logBuilder = new StringBuilder();
#if Debug
            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine(s);
            }
#else
            void LogMessage(String s) => logBuilder.AppendLine(string.Format("[{0:MM/dd/yyyy h:mm:ss.fff tt}] {1}", DateTime.Now, s));
#endif

            using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(repositories["Harvester"]))
            {
                LogMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                Operation operation = harvester.DataContext.Operations.FirstOrDefault(x => x.Name == operationContext.Operation.Name);

                if (operation == null)
                {
                    operation = new Operation {Name = operationContext.Operation.Name};

                    harvester.DataContext.Operations.InsertOnSubmit(operation);

                    harvester.DataContext.SubmitChanges();

                    operationContext.Operation.OperationID = operation.ID;
                }
                else
                {
                    operationContext.Operation.OperationID = operation.ID;
                    if (operation.OperationRecords.Any(x => x.RunDate.Date == operationContext.RunDate.Date))
                    {
                        skippedOperations++;
                        WriteQueueManagerChanges($"Operation {operationContext.Operation.Name.PadRight(33)} Has already Run");
                        harvester.DataContext.SubmitChanges();
                        return;
                    }
                }
            }

            LogMessage($"Starting operation '{operationContext.Operation.Name}' with run date of {operationContext.RunDate:MM/dd/yyyy h:mm:ss.fff tt}");

            TrackOperationsByDate(operationContext.Operation.OperationID);

            Task task = Task.Run(async () =>
            {
                source.Token.ThrowIfCancellationRequested();
                await IsTooManyOperationsRunning(operationContext, LogMessage);
                await ReachedMaximumRunsPerDay(operationContext, LogMessage);
                await IsTooManyConcurrentlyRunning(operationContext, LogMessage);

                lock (RunningOperations)
                {
                    RunningOperations.Add(operationContext);
                }
                WriteCurrentStatus();
                startTime = DateTime.Now;
                source.Token.ThrowIfCancellationRequested();
                WriteQueueManagerChanges($"operation {operationContext.Operation.Name.PadRight(33)} Began Executing");
                operationContext.Operation.Execute(operationContext.RunDate, LogMessage, source.Token);
            });

            Exception exception = null;

            try
            {
                LogMessage($"Awaiting operation {operationContext.Operation.Name}");
                await task;
            }
            catch (Exception ex)
            {
                exception = ex;
                LogMessage(ex.ToString());
            }
            finally
            {
                TotalOperationsRun++;
                lock (RunningOperations)
                {
                    RunningOperations.Remove(operationContext);
                }
                cancellationTokens.Remove(operationContext.Operation.Name + operationContext.RunDate.ToShortDateString());
                DateTime endDate = DateTime.Now;
                char statusLetter;

                switch (task.Status)
                {
                    case TaskStatus.Canceled:
                        WriteQueueManagerChanges($"operation {operationContext.Operation.Name.PadRight(33)} canceled");
                        LogMessage("The operation was cancelled.");
                        CanceledOperations.Add(operationContext);
                        statusLetter = 'C';
                        break;
                    case TaskStatus.Faulted:
                        WriteQueueManagerChanges($"operation {operationContext.Operation.Name.PadRight(33)} Faulted");
                        LogMessage("The operation encountered an error.");
                        FailedOperations.Add(operationContext);
                        statusLetter = 'F';
                        break;
                    case TaskStatus.RanToCompletion:
                        WriteQueueManagerChanges($"operation {operationContext.Operation.Name.PadRight(33)} Ran to Completion");
                        LogMessage("The operation completed successfully.");
                        SuccessfulOperations.Add(operationContext);
                        statusLetter = 'S';

                        using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(repositories["Harvester"]))
                        {
                            OperationRecord operationRecord = new OperationRecord
                            {
                                OperationID = operationContext.Operation.OperationID,
                                RunDate = operationContext.RunDate,
                                ExecutedDate = DateTime.Now
                            };
                            LogMessage($"Updated Harvester with Operation Record {JsonConvert.SerializeObject(operationRecord)}");
                            harvester.DataContext.OperationRecords.InsertOnSubmit(operationRecord);

                            harvester.DataContext.SubmitChanges();
                        }
                        break;
                    default:
                        WriteQueueManagerChanges($"operation {operationContext.Operation.Name} has an unexpected task status of: {task.Status}");
                        throw new NotImplementedException($"operation {operationContext.Operation.Name} has an unexpected task status of: {task.Status}");
                }

                RetriedOperations.Remove(operationContext);
                LogMessage("The operation took " + endDate.Subtract(startTime));

                string filename = $"{baseDirectory}Logs\\" +
                                  $"({statusLetter}) " +
                                  $"{operationContext.Operation.Name} " +
                                  $"({operationContext.RunDate:yyyy-MM-dd}) " +
                                  $"{(operationContext.CurrentRetry == 0 ? "" : "(" + operationContext.CurrentRetry + ")")} " +
                                  $"{Guid.NewGuid()}.txt";

                HarvesterService.WriteToFile(filename, logBuilder.ToString());
                logBuilder.Clear();

                WriteCurrentStatus();
                WriteOperations();

                switch (task.Status)
                {
#pragma warning disable CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    case TaskStatus.Faulted:
                        Task.Run(async () =>
                        {
                            if (exception?.GetType() == typeof(RepositoryIOException))
                            {
                                await Task.Delay(TimeSpan.FromSeconds(((RepositoryIOException) exception).RetryWaitTime), source.Token);
                            }
                            else
                            {
                                await Task.Delay(TimeSpan.FromDays(1), source.Token);
                            }
                            RetriedOperations.Add(operationContext);
                            RunOperation(operationContext.GetRetry());
                        });
                        break;
#pragma warning restore CS4014 // Because this call is not awaited, execution of the current method continues before the call is completed
                    case TaskStatus.RanToCompletion:
                    case TaskStatus.Canceled:
                        break;
                    default:
                        WriteQueueManagerChanges($"operation {operationContext.Operation.Name} has an unexpected task status of: {task.Status}");
                        throw new NotImplementedException($"operation {operationContext.Operation.Name} has an unexpected task status of: {task.Status}");
                }
            }
        }

        private void TrackOperationsByDate(int operationId)
        {
            lock (datedOperations)
            {
                string shortDate = DateTime.Now.ToShortDateString();
                if (datedOperations.ContainsKey(shortDate))
                {
                    if (!datedOperations[shortDate].ContainsKey(operationId))
                        datedOperations[shortDate].Add(operationId, 0);
                }
                else
                {
                    datedOperations.Add(shortDate, new Dictionary<int, int> { { operationId, 0 } });
                }
            }
        }

        private async Task ReachedMaximumRunsPerDay(OperationContext operationContext, Action<string> logMessage)
        {
            if (operationContext.MaximumRunsPerDay != 0)
            {
                TrackOperationsByDate(operationContext.Operation.OperationID);

                bool noAvailableRuns;
                lock (datedOperations)
                {
                    noAvailableRuns = datedOperations[DateTime.Now.ToShortDateString()][operationContext.Operation.OperationID] >= operationContext.MaximumRunsPerDay;
                }

                int postponedDays = 0;
                DateTime startDate = DateTime.Now;

                while (noAvailableRuns)
                {
                    postponedDays++;
                    requestsPostponedForADay++;
                    WriteCurrentStatus();
                    await Task.Delay(TimeSpan.FromDays(1));
                    requestsPostponedForADay--;
                    TrackOperationsByDate(operationContext.Operation.OperationID);
                    lock (datedOperations) 
                    {
                        noAvailableRuns = datedOperations[DateTime.Now.ToShortDateString()][operationContext.Operation.OperationID] >= operationContext.MaximumRunsPerDay;
                    }
                }

                if (postponedDays > 0)
                    logMessage(string.Format("Postponed operation '{0}' for {2} day(s) until {1:MM/dd/yyyy h:mm:ss.fff tt}", operationContext.Operation.Name, startDate.AddDays(postponedDays), postponedDays));

                lock (datedOperations)
                {
                    datedOperations[DateTime.Now.ToShortDateString()][operationContext.Operation.OperationID]++;
                    logMessage(string.Format("Operation '{0}' is running {2} out of {1} Maximum runs Allowed Daily", operationContext.Operation.Name, operationContext.MaximumRunsPerDay, datedOperations[DateTime.Now.ToShortDateString()][operationContext.Operation.OperationID]));
                }
            }
        }

        private async Task IsTooManyOperationsRunning(OperationContext operationContext, Action<string> logMessage)
        {
            int delayedMinutes = 0;
            DateTime startDate = DateTime.Now;
            while (Running >= maxRunningOperations)
            {
                delayedMinutes += minutesToDelayForMaxRunning;
                requestsDelayedForMaxRunning++;
                await Task.Delay(TimeSpan.FromMinutes(minutesToDelayForMaxRunning));
                requestsDelayedForMaxRunning--;
            }
            if (delayedMinutes > 0)
                logMessage(string.Format("Delayed operation '{0}' for {2} minutes until {1:MM/dd/yyyy h:mm:ss.fff tt} to reduce CPU Load", operationContext.Operation.Name, startDate.AddMinutes(delayedMinutes), delayedMinutes));
        }

        private async Task IsTooManyConcurrentlyRunning(OperationContext operationContext, Action<string> logMessage)
        {
            if (operationContext.MaximumConcurrentlyRunning != 0)
            {
                int minutesDelayed = 0;
                bool maximumOperationsRunning;
                lock (RunningOperations)
                {
                    maximumOperationsRunning = RunningOperations.Count(x => x.Operation.Name == operationContext.Operation.Name) >= operationContext.MaximumConcurrentlyRunning;
                }

                while (maximumOperationsRunning)
                {
                    minutesDelayed += minutesToDelayForConcurrency;
                    requestsDelayedForConcurrency++;
                    await Task.Delay(TimeSpan.FromMinutes(minutesToDelayForConcurrency));
                    requestsDelayedForConcurrency--;
                    lock (RunningOperations)
                    {
                        maximumOperationsRunning = RunningOperations.Count(x => x.Operation.Name == operationContext.Operation.Name) >= operationContext.MaximumConcurrentlyRunning;
                    }
                }
                if (minutesDelayed != 0)
                    logMessage($"Delayed operation '{operationContext.Operation.Name}' for {minutesDelayed} minutes until it could be run without any other operations of the same type running");
            }
        }

        public void StopQueueManager()
        {
            foreach (KeyValuePair<String, CancellationTokenSource> operation in cancellationTokens)
            {
                if (!operation.Value.IsCancellationRequested)
                {
                    operation.Value.Cancel();
                }
            }
        }

        #endregion

        #region Logging

        public void WriteCurrentStatus()
        {
            string currentStatus = 
                "Operations Run: "                                                + TotalOperationsRun                                           + "    \r\n" +
                "Successes: "                                                     + Successes                                                    + "    \r\n" +
                "Failures: "                                                      + Failures                                                     + "    \r\n" +
                "Running Operations: "                                            + Running                                            + "    \r\n" +
                "Retried Operations: "                                            + Retries                                                      + "    \r\n" +
                "Skipped Operations: "                                            + skippedOperations                                            + "    \r\n" +
                "Postponed Operations(1 day): "                                   + requestsPostponedForADay                                     + "    \r\n" +
                String.Format("Delayed Operations For Max Running({1} min): {0}", requestsDelayedForMaxRunning,  minutesToDelayForMaxRunning)    + "    \r\n" +
                String.Format("Delayed Operations For Concurrency({1} min): {0}", requestsDelayedForConcurrency, minutesToDelayForConcurrency)   + "    \r\n" +
                "Write Date: "                                                    + DateTime.Now                                                 + "    \r\n";

            try
            {
                HarvesterService.WriteToFile(baseDirectory + "Current Status.txt", currentStatus);
            }
            catch (Exception ex) 
            {
                if (ex.GetType() == typeof(IOException))
                {
                    HarvesterService.WriteEventLog(ex.Message, EventLogEntryType.Warning);
                }
                else
                {
                    throw;
                }
            }
        }

        private void WriteToBaseLog(string message)
        {
            string log = $"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}]----------------------------------------\n{message}";
            File.AppendAllText(baseDirectory + "BaseLog.txt", log);
            File.SetAccessControl(baseDirectory + "BaseLog.txt", FilePermissions.CreateFilePermissions());
        }

        private void WriteOperations()
        {
            try
            {
                StringBuilder builder = new StringBuilder();

                if (Running > 0)
                {
                    builder.AppendLine($"--------------------\nRunning Operations: {Running}\n--------------------");
                    lock (RunningOperations)
                    {
                        RunningOperations.GroupBy(x => x.Operation.Name).Select(x => new { Name = x.Key, Count = x.Count() }).ToList().ForEach(x => builder.AppendLine($"{x.Name}: {x.Count}"));
                    }
                }

                if (Successes > 0)
                {
                    builder.AppendLine($"--------------------\nSuccessful Operations: {Successes}\n--------------------");
                    SuccessfulOperations.GroupBy(x => x.Operation.Name).Select(x => new { Name = x.Key, Count = x.Count() }).ToList().ForEach(x => builder.AppendLine($"{x.Name}: {x.Count}"));
                }

                if (Failures > 0)
                {
                    builder.AppendLine($"--------------------\nFailed Operations: {Failures}\n--------------------");
                    FailedOperations.GroupBy(x => x.Operation.Name).Select(x => new { Name = x.Key, Count = x.Count() }).ToList().ForEach(x => builder.AppendLine($"{x.Name}: {x.Count}"));
                }
                builder.AppendLine();

                WriteToBaseLog(builder.ToString());
            }
            catch
            {
                // ignored
            }
        }

        private void WriteQueueManagerChanges(string message)
        {
            try
            {
                File.AppendAllText(baseDirectory + "QueueManager.txt", $"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}]: {message}\n");
                File.SetAccessControl(baseDirectory + "QueueManager.txt", FilePermissions.CreateFilePermissions());
            }
            catch
            {
                // ignored
            }
        }

        #endregion

        //#region ServiceHost

        //void NotifyRepositoryChange(object sender, SerializableNotifyCollectionChangedEventArgs a)
        //{
            
        //}

        //#endregion

        public event PropertyChangedEventHandler PropertyChanged;
        public Action<PropertyChangedEventArgs, NotifyCollectionChangedEventArgs> CollectionPropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private void OnCollectionChanged(PropertyChangedEventArgs p, NotifyCollectionChangedEventArgs e)
        {
            CollectionPropertyChanged?.Invoke(p, e);
        }
    }
}