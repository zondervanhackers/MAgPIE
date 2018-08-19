using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Core.Operations;
using ZondervanLibrary.Harvester.Core.Scheduling;
using ZondervanLibrary.Harvester.Service;
using System.Threading;
using System.Xml.Serialization;
using FileHelpers;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using ZondervanLibrary.Harvester.Core.Operations.Counter;
using ZondervanLibrary.Harvester.Core.Operations.Demographics;
using ZondervanLibrary.Harvester.Core.Operations.EZProxy;
using ZondervanLibrary.Harvester.Core.Operations.Statista;
using ZondervanLibrary.Harvester.Core.Operations.Sync;
using ZondervanLibrary.Harvester.Core.Operations.WmsInventory;
using ZondervanLibrary.Harvester.Core.Operations.WmsTransactions;
using ZondervanLibrary.Harvester.Core.Permissions;
using ZondervanLibrary.Harvester.Core.Repository.Email;
using ZondervanLibrary.Harvester.Entities;
using ItemType = ZondervanLibrary.Harvester.Core.Repository.Counter.ItemType;
using ConfigurationSettings = ZondervanLibrary.Harvester.Service.Configuration.ConfigurationSettings;

namespace Test
{
    // ReSharper disable once ClassNeverInstantiated.Global
    class Program
    {
        #if Local
        private static readonly SqlServerDatabaseRepositoryArguments StatisticsArguments = new SqlServerDatabaseRepositoryArguments
        {
            Name = "Statistics",
            Server = @"(localdb)\MSSQLLocalDB",
            Database = "Statistics",
            Authentication = SqlServerAuthenticationMethod.Windows
        };

        private static readonly SqlServerDatabaseRepositoryArguments HarvesterArguments = new SqlServerDatabaseRepositoryArguments
        {
            Name = "Harvester",
            Server = @"(localdb)\MSSQLLocalDB",
            Database = "Harvester",
            Authentication = SqlServerAuthenticationMethod.Windows
        };

        #else
        private static readonly SqlServerDatabaseRepositoryArguments StatisticsArguments = new SqlServerDatabaseRepositoryArguments
        {
            Name = "Statistics",
            Server = @"[Server]",
            Database = "Statistics",
            Authentication = SqlServerAuthenticationMethod.Windows
        };

        private static readonly SqlServerDatabaseRepositoryArguments HarvesterArguments = new SqlServerDatabaseRepositoryArguments
        {
            Name = "Harvester",
            Server = @"[Server]",
            Database = "Harvester",
            Authentication = SqlServerAuthenticationMethod.Windows
        };
        #endif

        private static readonly FolderDirectoryRepositoryArguments LocalJsonArguments = new FolderDirectoryRepositoryArguments
        {
            Name = "Json Local Copy",
            Path = @"C:\Harvester\JsonLocal"
        };

        private static void Main()
        {
            CreateExampleConfigurationFile(); 

            //TestImportDemographics();

            //TestFolder();

            //TestFtp();

            //TestSftp();

            //TestEbscoHost();

            //InsertOperationsIntoHarvester();
            //InsertRepositoriesIntoHarvester();

            //TestCounterFromConfigFile();

            //TestCounter();

            //TestJsonWriter();

            //TestWmsInventory();

            //TestDemographics();

            //TestWMSTransactions();

            //TestQueueManager();

            //SetLocalFilePermissions();

            //TestWmsInventorySync();

            //TestCancellation();

            //WriteQueueManagerToXml();

            //Test();

            //ImportStatistaOperation();

            //TestComparisonOfOperations();

            //TestEmailService();

            //TestImportEZProxyAudits();

            //TestImportEZProxyLogs();

            //TestQueueManagerStartup();

            //TestWebAutomation();

            //TestEbscoHostEmail();

            //TestImportEbscoEmailLocalFiles();

            //TestUpdatedDatabaseCommands();

            //WriteRecordsToJsonFile();
        }

        private static void CreateExampleConfigurationFile()
        {
            var exampleConfigurationFile = HarvesterService.CreateExampleConfigurationFile();

            using (StreamWriter stream = new StreamWriter("C:\\Harvester\\Configuration(Example).xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationSettings));

                serializer.Serialize(stream, exampleConfigurationFile);
            }
        }

        private static void TestWebAutomation()
        {
            var chromeOptions = new ChromeOptions();
            chromeOptions.AddArgument("test-type");
            chromeOptions.AddArguments("--allow-running-insecure-content");
            var chromeDriver = new ChromeDriver(@"C:\Users\timothy_parr\Downloads\chromedriver_win32", chromeOptions);
            chromeDriver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
            Actions actions = new Actions(chromeDriver);

            chromeDriver.Navigate().GoToUrl("http://eadmin.ebscohost.com/");
            chromeDriver.FindElementById("UserName").SendKeys("[Username]");
            chromeDriver.FindElementById("Password").SendKeys("[Password]");
            chromeDriver.FindElementById("Submit").Click();

            //Databases Tab
            chromeDriver.FindElementById("lnkDatabase").Click();

            var databaseNames = chromeDriver
                .FindElementById("grid_MainDataGrid")
                .FindElements(By.XPath("//*[@id=\"grid_MainDataGrid\"]/tbody/tr"))
                .Where((x, i) => i != 0)
                .Select(x => ParseDatabaseName(x.FindElements(By.TagName("td"))[2].Text)).Skip(2).ToList();

            //Reports Tab
            chromeDriver.FindElementById("custServiceHeader_toolbar_lnkReports").Click();

            chromeDriver.FindElementByLinkText("Standard Usage Reports").Click();

            var usageReportDropdown = new SelectElement(chromeDriver.FindElementById("Definitions"));
            usageReportDropdown.SelectByText("Title Usage Report");

            Thread.Sleep(3000);

            DateTime start = new DateTime(2016, 01, 01);
            DateTime end = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);

            //Export
            actions.MoveToElement(chromeDriver.FindElementById("emailReportRadio")).Click().Perform();
            chromeDriver.FindElementById("emailAddresses").SendKeys("[collectionEmail],[collectionEmail]");

            int successfulDatabases = databaseNames.Select((databaseName, i) =>
            {
                //Filters
                if (i == 0)
                    chromeDriver.FindElementById("allSubscribedRadio").Click();

                chromeDriver.FindElements(By.ClassName("btn")).First(x => x.Text == "Select/View").Click();

                chromeDriver.FindElementById("removeAll").Click();
                var options = chromeDriver.FindElementsByClassName("centerContents---2iwdV");
                options.FirstOrDefault(x => x.Text.Contains(databaseName)).FindElement(By.ClassName("select-button")).Click();

                chromeDriver.FindElementById("applyChangesButton").Click();

                //Options
                actions.MoveToElement(chromeDriver.FindElementById("detailed")).Click().Perform();

                var subjectInput = chromeDriver.FindElementById("emailSubject");
                subjectInput.Clear();
                subjectInput.SendKeys($"Title Usage Report - {databaseName}");

                new SelectElement(chromeDriver.FindElementById("emailReportFormat")).SelectByValue("tsv");

                DateTime date = start;
                while (date <= end)
                {
                    actions = new Actions(chromeDriver);
                    actions.MoveToElement(chromeDriver.FindElementById("datePickerButton")).Perform();
                    chromeDriver.FindElementById("datePickerButton").Click();

                    var customRangeButton = chromeDriver.FindElementByXPath("/html/body/div[3]/div[1]/ul/li[7]");
                    if (customRangeButton.Text != "Custom Range")
                        throw new FormatException();
                    customRangeButton.Click();
                    var calendars = chromeDriver.FindElementByClassName("daterangepicker");

                    var inputForStartDate = calendars.FindElements(By.ClassName("input-mini")).First(x => x.GetAttribute("name") == "daterangepicker_start");
                    var inputForEndDate = calendars.FindElements(By.ClassName("input-mini")).First(x => x.GetAttribute("name") == "daterangepicker_end");

                    inputForStartDate.Clear();
                    inputForStartDate.SendKeys(date.ToString("MM/dd/yyyy"));
                    inputForEndDate.Clear();
                    inputForEndDate.SendKeys(date.AddMonths(1).AddDays(-1).ToString("MM/dd/yyyy"));

                    chromeDriver.FindElementByClassName("applyBtn").Click();

                    actions.MoveToElement(chromeDriver.FindElementById("emailReport")).Build().Perform();
                    chromeDriver.FindElementById("emailReport").Click();

                    try
                    {
                        new WebDriverWait(chromeDriver, TimeSpan.FromMinutes(10)).Until(x =>
                        {
                            var reportBox = x.FindElement(By.Id("reportBox"));
                            return reportBox.GetAttribute("class") == "alert";
                        });
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Timed out on {databaseName} on {date:MM/dd/yyyy} ");
                        continue;
                    }

                    var badreportBox = chromeDriver.FindElement(By.Id("reportBox"));
                    if (badreportBox.Text.Contains("try again"))
                    {
                        Console.WriteLine($"FAILED for {databaseName} on {date:MM/dd/yyyy}");
                        Thread.Sleep(60000);
                        date = date.AddMonths(1);
                        continue;
                    }

                    try
                    {
                        new WebDriverWait(chromeDriver, TimeSpan.FromMinutes(10)).Until(x =>
                        {
                            var reportBox = x.FindElement(By.Id("reportBox"));
                            return reportBox.Text == "";
                        });
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Timed out on {databaseName} on {date:MM/dd/yyyy} ");
                    }

                    Console.WriteLine($"Sent Email for {databaseName} on {date:MM/dd/yyyy}");
                    date = date.AddMonths(1);

                    Thread.Sleep(15000);
                }

                //Finish with Monthly Frequency
                actions = new Actions(chromeDriver);
                actions.MoveToElement(chromeDriver.FindElementById("datePickerButton")).Perform();
                chromeDriver.FindElementById("datePickerButton").Click();

                var lastMonthButton = chromeDriver.FindElementByXPath("/html/body/div[3]/div[1]/ul/li[7]");
                if (lastMonthButton.Text != "Custom Range")
                    throw new FormatException();
                lastMonthButton.Click();

                chromeDriver.FindElementByClassName("applyBtn").Click();

                Console.WriteLine($"Scheduled Email set for {databaseName}");

                return true;
            }).Count();

            Console.WriteLine($"Created Reports for {successfulDatabases} databases.");
        }

        private static string ParseDatabaseName(string databaseName)
        {
            var lastParenthesis = databaseName.LastIndexOf("(");
            return databaseName.Substring(lastParenthesis).StartsWith("EBSCOhost") ? databaseName : databaseName.Remove(lastParenthesis - 1);
        }

        private static void WriteRecordsToJsonFile()
        {
            string deserializedRecords = File.ReadAllText("C:\\Harvester\\JsonLocal\\American Physical Society Import 2014-01-01.json");
            CounterRecord[] deserializeObject = JsonConvert.DeserializeObject<CounterRecord[]>(deserializedRecords);

            Console.WriteLine(deserializeObject);

            SushiCounterRepositoryArguments counterRepository = new SushiCounterRepositoryArguments
            {
                Name = "Sage Journals",
                Url = "http://journals.sagepub.com/api/soap/analytics/SushiService",
                RequestorID = "[RequestorID]",
                CustomerID = "[CustomerID]",
                ReleaseVersion = ReleaseVersion.R4,
                AvailableReports = new[] {CounterReport.JR1, CounterReport.JR2, CounterReport.JR3, CounterReport.PR1},
                JsonRepository = LocalJsonArguments,
            };

            ImportCounterTransactionsOperation operation = new ImportCounterTransactionsOperation(StatisticsArguments, HarvesterArguments, counterRepository, LocalJsonArguments) { Name = "American Physical Society Import" };

            DateTime date = DateTime.Now;
            StringBuilder logBuilder = new StringBuilder();

            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine("[{0:MM/dd/yyyy h:mm:ss.fff tt}] {1}", DateTime.Now, s);
            }

            Console.WriteLine("==== EZProxy Audit Import Operation ===============");
            operation.Execute(new DateTime(2016, 5, 1), LogMessage, new CancellationToken());
            Console.WriteLine("Completed Date: " + date);
            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestUpdatedDatabaseCommands()
        {
            using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(HarvesterArguments))
            {
                //harvester.DataContext.Repositories.InsertOnSubmit(new Repository
                //{
                //    RepositoryName = "TestRepo",
                //    GUID = new Guid("10F7F3A4-4187-46C3-8976-1A37381353BA")//Guid.NewGuid()
                //});

                //harvester.DataContext.Operations.InsertOnSubmit(new Operation
                //{
                //    OperationName = "TestOperation",
                //    GUID = Guid.NewGuid(),

                //});

                //harvester.DataContext.DirectoryRecords.InsertOnSubmit(new DirectoryRecord
                //{
                //   FileModifiedDate = DateTime.Now,
                //   FilePath = "/Filename.txt",
                //   OperationID = 1,
                //   RepositoryID = 2
                //});

                DirectoryRecord directoryRecords = harvester.DataContext.DirectoryRecords.First(x => x.Repository.Name == "TestRepo" && x.Operation.Name == "TestOperation");
                directoryRecords.ModifiedDate = DateTime.Now;

                harvester.DataContext.SubmitChanges();
            }
        }

        private static void TestEbscoHostEmail()
        {
            EmailRepositoryArguments emailRepositoryArguments = new EmailRepositoryArguments
            {
                Name = "EbscoHostEmail",
                HostName = "imap.gmail.com",
                Username = "[Email]",
                Password = "[Password]",
                Port = 993,
                UseSsl = true,
                FromAddress = "[From Address]",
                JsonRepository = LocalJsonArguments
            };

            EmailRepositoryArguments fatCowEmail = new EmailRepositoryArguments
            {
                Name = "fatcowEmail",
                HostName = "[Email Host]",
                Username = "[Email]",
                Password = "[Password]",
                Port = 143,
                UseSsl = false,
                FromAddress = "[From Address]",
                JsonRepository = LocalJsonArguments
            };

            ImportCounterTransactionsOperation operation = new ImportCounterTransactionsOperation(StatisticsArguments, HarvesterArguments, emailRepositoryArguments, LocalJsonArguments) {Name = "ImportEbscoHost"};

            StringBuilder logBuilder = new StringBuilder();

            void LogMessage(string s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine(s);
            }

            Console.WriteLine($"==== {operation.Name} Operation ===============");
            DateTime date = new DateTime(2016, 2, 1);
            while (date < new DateTime(2018, 4, 1))
            {
                operation.Execute(date, LogMessage, new CancellationToken());
                Console.WriteLine("Completed Date: " + date);
                using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
                {
                    writer.Write(logBuilder.ToString());
                    logBuilder.Clear();
                }

                date = date.AddMonths(1);
            }
        }

        private static void TestImportEbscoEmailLocalFiles()
        {
            var delimitedFileEngine = new DelimitedFileEngine<EbscoEmailRecord> { Options = { IgnoreFirstLines = 8, IgnoreEmptyLines = true } };

            List<CounterRecord> records = new List<CounterRecord>();

            string vendorName = "EbscoHost";
            var message = new { Database = "", Date = DateTime.Now, Data = "" };

            var databaseRecord = new CounterRecord { ItemName = message.Database, ItemType = ItemType.Database, ItemPlatform = vendorName, RunDate = message.Date, Identifiers = new[] { new CounterIdentifier { IdentifierType = IdentifierType.Database, IdentifierValue = vendorName } }, Metrics = new CounterMetric[] { } };
            var vendorRecord = new CounterRecord { ItemName = vendorName, ItemType = ItemType.Vendor, RunDate = message.Date, Identifiers = new[] { new CounterIdentifier { IdentifierType = IdentifierType.Proprietary, IdentifierValue = vendorName } }, Metrics = new CounterMetric[] { } };

            foreach (var record in delimitedFileEngine.ReadStringAsList(message.Data))
            {
                records.Add(new CounterRecord
                {
                    ItemName = record.Title,
                    ItemType = record.ResourceType,
                    RunDate = message.Date,
                    ItemPlatform = message.Database,
                    Identifiers = new[]
                    {
                            new CounterIdentifier
                            {
                                IdentifierType = IdentifierType.PrintIssn,
                                IdentifierValue = record.Print_ISSN
                            },
                            new CounterIdentifier
                            {
                                IdentifierType = IdentifierType.OnlineIssn,
                                IdentifierValue = record.Online_ISSN
                            },
                            new CounterIdentifier
                            {
                                IdentifierType = IdentifierType.Proprietary,
                                IdentifierValue = record.Proprietary_Identifier
                            },
                            new CounterIdentifier
                            {
                                IdentifierType = IdentifierType.Isbn,
                                IdentifierValue = record.ISBN
                            },

                        },
                    Metrics = new[]
                    {
                            new CounterMetric
                            {
                                MetricType = MetricType.FullTextTotalRequests,
                                MetricValue = record.Total_Full_Text_Requests
                            }
                        },
                });
            }
        }

        private static void TestQueueManagerStartup()
        {
            QueueManager manager = new QueueManager(WriteConfigFile(), "C:\\Harvester\\");
            manager.CheckQueue();
        }

        private static void TestImportEZProxyLogs()
        {
            ImportEZProxyLogOperationArguments args = new ImportEZProxyLogOperationArguments
            {
                DestinationDatabase = "Statistics",
                HarvesterDatabase = "Harvester",
                Name = "EZProxy Log Import",
            };

            FolderDirectoryRepositoryArguments folderArgs = new FolderDirectoryRepositoryArguments
            {
                Name = "EZProxy Logs",
                Path = "C:\\Harvester\\EZProxy Logs",
            };

            ImportEZProxyLogOperation operation = new ImportEZProxyLogOperation(args, HarvesterArguments, StatisticsArguments, folderArgs) { Name = "EZProxy Log Import" };
            DateTime date = DateTime.Now;
            StringBuilder logBuilder = new StringBuilder();
            void LogMessage(String s) => logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
            Console.WriteLine("==== EZProxy Log Import Operation ===============");
            operation.Execute(date, LogMessage, new CancellationToken());
            Console.WriteLine("Completed Date: " + date);
            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestImportEZProxyAudits()
        {
            ImportEZProxyAuditOperationArguments args = new ImportEZProxyAuditOperationArguments
            {
                DestinationDatabase = "Statistics",
                HarvesterDatabase = "Harvester",
                Name = "EZProxy Import",
                FilePattern = @"\d{8}[.]txt",
            };

            FolderDirectoryRepositoryArguments logFolderArgs = new FolderDirectoryRepositoryArguments
            {
                Name = "EZProxy Logs",
                Path = "C:\\Harvester\\EZProxy Logs",
            };

            FolderDirectoryRepositoryArguments sourceFolderArgs = new FolderDirectoryRepositoryArguments
            {
                Name = "EZProxy Audit",
                Path = "C:\\Harvester\\EZProxy Audits",
            };

            ImportEZProxyAuditOperation operation = new ImportEZProxyAuditOperation(args, HarvesterArguments, StatisticsArguments, sourceFolderArgs, logFolderArgs);
            DateTime date = DateTime.Now;
            StringBuilder logBuilder = new StringBuilder();

            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine("[{0:MM/dd/yyyy h:mm:ss.fff tt}] {1}", DateTime.Now, s);
            }

            Console.WriteLine("==== EZProxy Audit Import Operation ===============");
            operation.Execute(date, LogMessage, new CancellationToken());
            Console.WriteLine("Completed Date: " + date);
            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestEmailService()
        {
            HarvesterService.SendEmail("Hello Tim", new[] {"[Test Email]"});
            Console.WriteLine();
        }

        private static void TestComparisonOfOperations()
        {
            OperationArgumentsBase args = new SyncOperationArguments
            {
                Name = "SyncDirectories",
                SourceDirectory = "Oclc",
                DestinationDirectory = "WMSInventory",
                Schedules = new ScheduleArgumentsBase[]
                {
                    new IntervalScheduleArguments
                    {
                        Interval = 1,
                        Unit = IntervalUnit.Weeks,
                        StartDate = DateTime.Now
                    }
                },
                FilePattern = @"^ITU[.]Item_inventories[.]\d{8}[.]txt$",
            };

            OperationArgumentsBase otherArgs = new SyncOperationArguments
            {
                Name = "SyncDirectories",
                SourceDirectory = "sfesef",
                DestinationDirectory = "zefs",
                Schedules = new ScheduleArgumentsBase[]
                {
                    new IntervalScheduleArguments
                    {
                        Interval = 1,
                        Unit = IntervalUnit.Weeks,
                        StartDate = DateTime.Now
                    }
                },
                FilePattern = @"^ITU[.]Item_inventories[.]\d{8}[.]txt$",
            };

            Console.WriteLine(args.Equals(otherArgs));
            Console.WriteLine();
        }

        private static void ImportStatistaOperation()
        {
            ImportStatistaOperationArguments args = new ImportStatistaOperationArguments
            {
                DestinationDatabase = "Statistics",
                HarvesterDatabase = "Harvester",
                FilePattern = @"(Taylor University [(]\d{4}\-\d{2}[)](\.xlsx))",
                Name = "Statista Import"
            };

            FolderDirectoryRepositoryArguments folderArgs = new FolderDirectoryRepositoryArguments
            {
                Name = "Statista Folder",
                Path = "C:\\Harvester\\Statista"
            };
            ImportStatistaOperation operation = new ImportStatistaOperation(args, HarvesterArguments, StatisticsArguments, folderArgs) {Name = args.Name, OperationID = 11};
            DateTime date = DateTime.Now;
            StringBuilder logBuilder = new StringBuilder();
            void LogMessage(String s) => logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
            Console.WriteLine("==== Statista Import Operation ===============");
            operation.Execute(date, LogMessage, new CancellationToken());
            Console.WriteLine("Completed Date: " + date);
            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestCancellation()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            Task task = Task.Factory.StartNew(() =>
            {
                Int32 i = 0;
                try
                {
                    while (true)
                    {
                        Thread.Sleep(2000);

                        cts.Token.ThrowIfCancellationRequested();

                        i++;

                        if (i > 5)
                            throw new InvalidOperationException();
                    }
                }
                catch
                {
                    Console.WriteLine("i = {0}", i);
                    throw;
                }
            }, cts.Token);

            task.ContinueWith(t => Console.WriteLine("{0} with {1}: {2}",
                    t.Status, t.Exception?.InnerExceptions[0].GetType(), t.Exception?.InnerExceptions[0].Message),
                TaskContinuationOptions.OnlyOnFaulted);
            task.ContinueWith(t => Console.WriteLine(t.Status), TaskContinuationOptions.OnlyOnCanceled);

            Console.ReadLine();

            cts.Cancel();

            Console.ReadLine();
        }

        private static void SetLocalFilePermissions()
        {
            foreach (String file in Directory.EnumerateFiles("C:\\Harvester\\tmpDemographics\\"))
            {
                File.SetAccessControl(file, FilePermissions.CreateFilePermissions());
            }
        }

        private static void InsertOperationsIntoHarvester()
        {
            var configSettings = HarvesterService.CreateExampleConfigurationFile();

            using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(HarvesterArguments))
            {
                var operations = configSettings.Operations.Where(x => harvester.DataContext.Operations.Count(y => y.Name == x.Name) == 0).Select(x => new Operation() { Name = x.Name });
                harvester.DataContext.Operations.InsertAllOnSubmit(operations);
                harvester.DataContext.SubmitChanges();
            }
        }

        private static void InsertRepositoriesIntoHarvester()
        {
            var configSettings = HarvesterService.CreateExampleConfigurationFile();

            using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(HarvesterArguments))
            {
                var operations = configSettings.Repositories.Where(x => harvester.DataContext.Repositories.Count(y => y.Name == x.Name) == 0).Select(x => new Repository() { Name = x.Name });
                harvester.DataContext.Repositories.InsertAllOnSubmit(operations);
                harvester.DataContext.SubmitChanges();
            }
        }

        private static void TestCounterFromConfigFile()
        {
            var configSettings = HarvesterService.CreateExampleConfigurationFile();
            OperationContextFactory.SetRepositories(configSettings.Repositories);

            foreach (ImportCounterTransactionsOperationArguments operation in configSettings.Operations.Where(x => x is ImportCounterTransactionsOperationArguments))
            {
                var schedule = new IntervalSchedule(operation.Schedules.First() as IntervalScheduleArguments).GetEnumerator();
                schedule.MoveNext();
                var operationContext = OperationContextFactory.CreateOperationContext(operation, schedule);

                StringBuilder logBuilder = new StringBuilder();

                void LogMessage(String s)
                {
                    logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                    Console.WriteLine(s);
                }

                Console.WriteLine("==== {0} Operation ===============", operationContext.Operation.Name);
                operationContext.Operation.Execute(operationContext.RunDate, LogMessage, new CancellationToken());
                Console.WriteLine("Completed Date: " + operationContext.RunDate);
                using (StreamWriter writer =
                    new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
                {
                    writer.Write(logBuilder.ToString());
                    logBuilder.Clear();
                }
            }
        }

        private static void TestCounter()
        {
            var sushiArgs = new SushiMuseCounterRepositoryArguments
            {
                Name = "Lexis Nexis",
                Url = "http://usage.lexisnexis.com/SushiServer",
                RequestorID = "bleh",
                CustomerID = "[CustomerID]",
                ReleaseVersion = ReleaseVersion.R3,
                AvailableReports = new[] { CounterReport.DB3 },
                JsonRepository = LocalJsonArguments
            };

            DateTime date = new DateTime(2011, 10, 1);
            //while (date <= new DateTime(2018, 1, 1))
            {
                ImportCounterTransactionsOperation operation = new ImportCounterTransactionsOperation(StatisticsArguments, HarvesterArguments, sushiArgs, LocalJsonArguments) { Name = "Lexis Nexis Counter" };
                StringBuilder logBuilder = new StringBuilder();

                void LogMessage(String s)
                {
                    logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                    Console.WriteLine(s);
                }

                Console.WriteLine("==== {0} Operation ===============", sushiArgs.Name);
                operation.Execute(date, LogMessage, new CancellationToken());
                Console.WriteLine("Completed Date: " + date);
                using (StreamWriter writer =
                    new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
                {
                    writer.Write(logBuilder.ToString());
                    logBuilder.Clear();
                }

                date = date.AddMonths(1);
            }
            Console.WriteLine("Finished");
        }

        private static void TestWmsInventory()
        {
            //TestWmsInventorySync();

            RepositoryArgumentsBase folderArgs = new FolderDirectoryRepositoryArguments { Path = @"C:\Harvester\WMSInventory", Name = "WMSInventory" };
            DateTime date = DateTime.Now;

            ImportWmsInventoryOperation operation = new ImportWmsInventoryOperation(HarvesterArguments, StatisticsArguments, folderArgs) { Name = "WMS Inventory Import" };
            StringBuilder logBuilder = new StringBuilder();

            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine(s);
            }

            try
            {
                operation.Execute(date, LogMessage, new CancellationToken());
            }
            catch { }

            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestWMSTransactionSync()
        {
            RepositoryArgumentsBase folderArgs = new FolderDirectoryRepositoryArguments
            {
                Name = "WMSTransaction",
                Path = @"C:\Harvester\WMSTransactions",
            };

            SftpDirectoryRepositoryArguments sftpArgs = new SftpDirectoryRepositoryArguments
            {
                Name = "Oclc Transactions",
                Host = "ftp2",
                Port = 22,
                Username = "[Username]",
                Password = "[Password]",
                DirectoryPath = "[Path/Path/]"
            };

            SyncOperationArguments syncArgs = new SyncOperationArguments
            {
                Name = "Sync WMSTransactions",
                SourceDirectory = "Oclc Transactions",
                DestinationDirectory = "WMSTransaction",
                Schedules = new ScheduleArgumentsBase[]
                {
                    new IntervalScheduleArguments
                    {
                        Interval = 1,
                        Unit = IntervalUnit.Weeks,
                        StartDate = DateTime.Now.AddHours(3)
                    }
                },
                FilePattern = @"^Daily_Transaction_Export_for_VAMP.*[.](csv|txt)$"
            };

            StringBuilder logBuilder = new StringBuilder();

            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine(s);
            }

            SyncOperation syncOperation = new SyncOperation(syncArgs, folderArgs, sftpArgs);
            syncOperation.Execute(DateTime.Now, LogMessage, new CancellationToken());

            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestWMSTransactions()
        {
            TestWMSTransactionSync();

            RepositoryArgumentsBase folderArgs = new FolderDirectoryRepositoryArguments
            {
                Name = "WMSTransaction",
                Path = @"C:\Harvester\WMSTransactions",
            };

            DateTime date = DateTime.Now;

            StringBuilder logBuilder = new StringBuilder();

            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine(s);
            }

            ImportWmsTransactionOperation operation = new ImportWmsTransactionOperation(HarvesterArguments, StatisticsArguments, folderArgs) {OperationID = 14};
            operation.Execute(date, LogMessage, new CancellationToken());

            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestDemographics()
        {
            RepositoryArgumentsBase folderArgs = new FolderDirectoryRepositoryArguments {Path = @"C:\Harvester\Demographics", Name = "Demographics"};

            ImportDemographicsOperation operation = new ImportDemographicsOperation(HarvesterArguments, StatisticsArguments, folderArgs);

            StringBuilder logBuilder = new StringBuilder();

            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine(s);
            }

            DateTime date = new DateTime(2015, 01, 01);
            try
            {
                operation.Execute(date, LogMessage, new CancellationToken());
            }
            catch (Exception ex)
            {
                LogMessage(ex.ToString());
            }
            finally
            {
                using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
                {
                    writer.Write(logBuilder.ToString());
                    logBuilder.Clear();
                }
            }
            Console.ReadLine();
        }

        private static ConfigurationSettings WriteConfigFile()
        {
            ConfigurationSettings configFile = HarvesterService.CreateExampleConfigurationFile();

            using (StreamWriter stream = new StreamWriter("C:\\Harvester\\Configuration.xml"))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(ConfigurationSettings));

                serializer.Serialize(stream, configFile);

                return HarvesterService.CreateExampleConfigurationFile();
            }
        }

        private static void TestImportDemographics()
        {
            RepositoryArgumentsBase folderArgs = new FolderDirectoryRepositoryArguments { Path = @"C:\Harvester\Demographics", Name = "Demographics" };

            ImportDemographicsOperationArguments args = new ImportDemographicsOperationArguments
            {
                Name = "DemographicImport",
                SourceDirectory = "Demographics",
                DestinationDatabase = "Statistics",
                HarvesterDatabase = "Harvester",
                Schedules = new ScheduleArgumentsBase[]
                {
                    new IntervalScheduleArguments
                    {
                        Interval = 7,
                        Unit = IntervalUnit.Weeks,
                        StartDate = new DateTime(2016, 2, 1)
                    }
                }
            };

            ImportDemographicsOperation operation = new ImportDemographicsOperation(HarvesterArguments, StatisticsArguments, folderArgs);
            StringBuilder logBuilder = new StringBuilder();
            void LogMessage(String s) => logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");

            operation.Execute(DateTime.Now, LogMessage, new CancellationToken());
            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
        }

        private static void TestQueueManager()
        {
            try
            {
                ConfigurationSettings modifiedConfiguration = WriteConfigFile();
                QueueManager _queueManager = new QueueManager(modifiedConfiguration, "C:\\Harvester\\");

                ImportCounterTransactionsOperationArguments modifiedArgument = new ImportCounterTransactionsOperationArguments
                {
                    Name = "AcsImport",
                    SourceCounter = "AmericanChemicalSociety",
                    DestinationDatabase = "Statistics",
                    Schedules = new ScheduleArgumentsBase[]
                    {
                        new IntervalScheduleArguments
                        {
                            Interval = 1,
                            Unit = IntervalUnit.Months,
                            StartDate = new DateTime(2014, 1, 1),
                            EndDate = new DateTime(2016, 8, 1)
                        }
                    },
                    MaximumRunsPerDay = 16
                };

                List<OperationArgumentsBase> operationsList = modifiedConfiguration.Operations.ToList();
                operationsList.Remove(modifiedArgument);
                modifiedArgument.MaximumRunsPerDay = 16;
                operationsList.Add(modifiedArgument);
                modifiedConfiguration.Operations = operationsList.ToArray();
                _queueManager.UpdateConfigurationFile(modifiedConfiguration);
            }
            catch (IOException exception)
            {
                Console.WriteLine(exception.ToString());
                throw;
            }
        }

        private static void TestSftp()
        {
            SftpDirectoryRepositoryArguments arguments = new SftpDirectoryRepositoryArguments
            {
                //Name = "Oclc",
                //Host = "",
                //Port = 22,
                //Username = "",
                //Password = "",
                //DirectoryPath = ""
            };

            using (SftpDirectoryRepository repository = new SftpDirectoryRepository(arguments))
            {
                foreach (DirectoryObjectMetadata fileData in repository.ListFiles("/"))
                {
                    Console.WriteLine("Name: {0}, Path: {1}, Type: {2}, Modified: {3}", fileData.Name, fileData.Path, fileData.ObjectType, fileData.ModifiedDate);
                }
            }
            Console.ReadLine();
        }

        private static void TestFtp()
        {
            FtpDirectoryRepositoryArguments arguments = new FtpDirectoryRepositoryArguments
            {
                //Host = "",
                //Port = 21,
                //Username = "",
                //Password = "",
                //UseSsl = false
            };

            using (FtpDirectoryRepository repository = new FtpDirectoryRepository(arguments))
            {
                foreach (DirectoryObjectMetadata @object in repository.ListFiles("/"))
                {
                    Console.WriteLine("Name: {0}, Path: {1}, Type: {2}, Modified: {3}", @object.Name, @object.Path, @object.ObjectType, @object.ModifiedDate);
                }
            }
        }

        private static void TestFolder()
        {
            FolderDirectoryRepositoryArguments arguments = new FolderDirectoryRepositoryArguments
            {
                Path = @"C:\Users\micah_hahn\Desktop\Demographics\"
            };

            using (FolderDirectoryRepository repository = new FolderDirectoryRepository(arguments))
            {
                foreach (DirectoryObjectMetadata @object in repository.ListFiles("/"))
                {
                    Console.WriteLine("Name: {0}, Path: {1}, Type: {2}, Modified: {3}", @object.Name, @object.Path, @object.ObjectType, @object.ModifiedDate);
                }
            }
        }

        private static void TestEbscoHost()
        {
            EbscoHostCounterRepositoryArguments repoArgs = new EbscoHostCounterRepositoryArguments
            {
                Name = "EbscoHost",
                Username = "[Username]",
                Password = "[Password]",
                Sushi = new SushiCounterRepositoryArguments
                {
                    Url = "http://sushi.ebscohost.com/R4/SushiService.svc",
                    RequestorID = "[RequestorID]",
                    CustomerID = "[CustomerID]",
                    ReleaseVersion = ReleaseVersion.R4,
                    AvailableReports = new[] {CounterReport.JR1, CounterReport.JR2, CounterReport.JR3, CounterReport.BR1, CounterReport.BR3}
                }
            };

            DateTime date = new DateTime(2016, 01, 01);
            //while (date <= new DateTime(2017, 01, 01))
            {
                ImportCounterTransactionsOperation operation = new ImportCounterTransactionsOperation(StatisticsArguments, HarvesterArguments, repoArgs, LocalJsonArguments);
                StringBuilder logBuilder = new StringBuilder();
                void LogMessage(String s) => logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine("==== EbscoHost Operation ===============");
                operation.Execute(date, LogMessage, new CancellationToken());
                Console.WriteLine("Completed Date: " + date);
                using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
                {
                    writer.Write(logBuilder.ToString());
                }
                date = date.AddMonths(1);
            }
            Console.WriteLine();
        }

        private static void TestWmsInventorySync()
        {
            SyncOperationArguments operationArgs = new SyncOperationArguments
            {
                Name = "SyncDirectories",
                SourceDirectory = "Oclc",
                DestinationDirectory = "WMSInventory",
                Schedules = new ScheduleArgumentsBase[]
                {
                    new IntervalScheduleArguments
                    {
                        Interval = 1,
                        Unit = IntervalUnit.Weeks,
                        StartDate = DateTime.Now
                    }
                },
                FilePattern = @"^(ITU[.]Item_inventories[.]\d{8}[.]txt)|(ITU[.]Circulation_Item_Inventories[.]\d{8}[.]txt([.]zip)?)$",
            };

            SftpDirectoryRepositoryArguments sourceArgs = new SftpDirectoryRepositoryArguments
            {
                //Name = "Oclc",
                //Host = "",
                //Port = 22,
                //Username = "",
                //Password = "",
                //DirectoryPath = ""
            };

            FolderDirectoryRepositoryArguments targetArgs = new FolderDirectoryRepositoryArguments
            {
                Name = "WMSInventory",
                Path = "C:\\Harvester\\WMSInventory\\"
            };

            SyncOperation operation = new SyncOperation(operationArgs, targetArgs, sourceArgs);
            StringBuilder logBuilder = new StringBuilder();
            void LogMessage(String s)
            {
                logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
                Console.WriteLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");
            }

            operation.Execute(DateTime.Now, LogMessage, new CancellationToken());
            using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
            {
                writer.Write(logBuilder.ToString());
                logBuilder.Clear();
            }
            //Console.ReadLine();
        }

        private static void Test()
        {
            ImportCounterTransactionsOperationArguments operationArguments = new ImportCounterTransactionsOperationArguments
            {
                Name = "Oxford",
                DestinationDatabase = "Statistics",
                SourceCounter = "Oxford",
                Schedules = new ScheduleArgumentsBase[]
                {
                    new IntervalScheduleArguments
                    {
                        Interval = 1,
                        Unit = IntervalUnit.Months,
                        StartDate = new DateTime(2014, 3, 1),
                        EndDate = new DateTime(2016, 8, 1)
                    }
                },
            };

            SushiCounterRepositoryArguments sushArguments = new SushiCounterRepositoryArguments
            {
                Url = "http://sushi4.scholarlyiq.com/SushiService.svc",
                CustomerID = "[CustomerID]",
                RequestorID = "[RequestorID]",
                Name = "Oxford",
                ReleaseVersion = ReleaseVersion.R4,
                AvailableReports = new[] {CounterReport.BR2, CounterReport.PR1},
            };

            DateTime runDate = new DateTime(2014, 01, 01);
            while (runDate <= new DateTime(2014, 12, 01))
            {
                ImportCounterTransactionsOperation operation = new ImportCounterTransactionsOperation(StatisticsArguments, HarvesterArguments, sushArguments, LocalJsonArguments);

                StringBuilder logBuilder = new StringBuilder();
                void LogMessage(String s) => logBuilder.AppendLine($"[{DateTime.Now:MM/dd/yyyy h:mm:ss.fff tt}] {s}");

                Console.WriteLine("==== {0} Operation ===============", operation.Name);
                operation.Execute(runDate, LogMessage, new CancellationToken());
                Console.WriteLine("Completed Date: " + runDate);

                using (StreamWriter writer = new StreamWriter($@"C:\Harvester\Logs\{Guid.NewGuid()}.txt"))
                {
                    writer.Write(logBuilder.ToString());
                    logBuilder.Clear();
                }
                runDate = runDate.AddMonths(1);
            }
        }
    }
}