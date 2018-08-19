using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;
using FileHelpers;
using HtmlAgilityPack;
using ImapX;
using ImapX.Enums;
using Newtonsoft.Json;
using ZondervanLibrary.Harvester.Core.Exceptions;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Directory;

#pragma warning disable 649
namespace ZondervanLibrary.Harvester.Core.Repository.Email
{
    public partial class EmailRepository : RepositoryBase, ICounterRepository
    {
        private readonly EmailRepositoryArguments arguments;
        private readonly ImapClient client;

        public EmailRepository(EmailRepositoryArguments _arguments)
        {
            arguments = _arguments;
            Name = _arguments.Name;

            client = new ImapClient(arguments.HostName, arguments.Port, arguments.UseSsl);
            client.Connect();

            if (!client.Login(_arguments.Username, _arguments.Password))
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidCredentials, this, "Failed to Connect to " + Name);
            }
        }

        /// <inheritdoc />
        public IEnumerable<CounterRecord> RequestRecords(DateTime runDate, CounterReport report)
        {
            (string fileName, EmailRecord emailRecord)[] downloadMessages = DownloadMessages().ToArray();

            //Process Raw Files just downloaded
            if (arguments.JsonRepository != null)
            {
                foreach (EmailRecord emailRecord in downloadMessages.Select(x => x.emailRecord))
                {
                    Console.WriteLine("Data Length: " + emailRecord.Data.Length);
                    SaveJsonFile(emailRecord);
                }
            }

            if (arguments.JsonRepository == null)
            {
                yield break;
            }

            //Process all unprocessed Files
            using (IDirectoryRepository localJsonRepo = RepositoryFactory.CreateDirectoryRepository(arguments.JsonRepository))
            {
                var ebscoEmailFiles = localJsonRepo.ListFiles().Where(x => x.Name.StartsWith($"{Name}")).ToList();

                var rawEbscoEmailFiles = ebscoEmailFiles.Where(x => x.Name.EndsWith(".tsv")).ToList();
                var processedEbscoEmailFiles = ebscoEmailFiles.Where(x => x.Name.EndsWith(".json")).ToList();

                foreach (var file in rawEbscoEmailFiles)
                {
                    if (processedEbscoEmailFiles.Any(x => x.Name.StartsWith(file.Name.Replace(".tsv", ""))))
                        continue;

                    string rawData = new StreamReader(localJsonRepo.OpenFile(file.Name)).ReadToEnd();
                    SaveJsonFile(GetEmailRecordFromFile(file.Name, rawData));
                    LogMessage($"Created local Json file from {file.Name}");
                }
            }

            //Return Records
            using (IDirectoryRepository localJsonRepo = RepositoryFactory.CreateDirectoryRepository(arguments.JsonRepository))
            {
                DirectoryObjectMetadata ebscoEmailReportFile = localJsonRepo.ListFiles().FirstOrDefault(x => x.Name.StartsWith($"{Name} {runDate:yyyy-MM-dd} {report}") && x.Name.EndsWith(".json"));

                if (ebscoEmailReportFile != null)
                {
                    using (Stream ebscoEmailReportStream = localJsonRepo.OpenFile(ebscoEmailReportFile.Name))
                    {
                        string jsonString = new StreamReader(ebscoEmailReportStream).ReadToEnd();
                        var counterRecords = JsonConvert.DeserializeObject<CounterRecord[]>(jsonString);
                        foreach (CounterRecord record in counterRecords)
                            yield return record;
                    }
                }
                else
                {
                    var ebscoEmailFiles = localJsonRepo.ListFiles().Where(x => x.Name.StartsWith($"{Name} {runDate:yyyy-MM-dd}") && x.Name.EndsWith(".json"));

                    foreach (DirectoryObjectMetadata ebscoEmailFile in ebscoEmailFiles)
                    {
                        using (Stream ebscoEmailReportStream = localJsonRepo.OpenFile(ebscoEmailFile.Name))
                        {
                            string jsonString = new StreamReader(ebscoEmailReportStream).ReadToEnd();
                            var counterRecords = JsonConvert.DeserializeObject<CounterRecord[]>(jsonString);
                            foreach (CounterRecord record in counterRecords)
                                yield return record;
                        }

                        LogMessage($"Harvested {ebscoEmailFile.Name}");
                    }
                }
            }
        }

        private EmailRecord GetEmailRecordFromFile(string fileName, string data)
        {
            Regex fileNameRegex = new Regex($@"^{Name} (?<date>\d{{4}}-\d{{2}}-\d{{2}}) (?<database>.+)\.tsv$");

            var matches = fileNameRegex.Match(fileName);
            DateTime date = DateTime.ParseExact(matches.Groups["date"].Value, "yyyy-MM-dd", null);
            string database = matches.Groups["database"].Value;

            return new EmailRecord(database, data, date);
        }

        private void SaveJsonFile(EmailRecord emailRecord)
        {
            Console.WriteLine("Data Length: " + emailRecord.Data.Length);
            var counterRecords = ParseMessages(new[] { emailRecord }).ToArray();

            using (IDirectoryRepository directoryRepo = RepositoryFactory.CreateDirectoryRepository(arguments.JsonRepository))
            {
                string jsonFileName = $"{Name} {emailRecord.Date:yyyy-MM-dd} {emailRecord.Database}.json";

                using (Stream stream = directoryRepo.CreateFile(jsonFileName, FileCreationMode.Overwrite))
                {
                    JsonSerializer serializer = JsonSerializer.Create();
                    using (StreamWriter streamWriter = new StreamWriter(stream))
                    {
                        using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter))
                        {
                            serializer.Serialize(jsonTextWriter, counterRecords);
                        }
                    }
                }

                LogMessage($"Created File {jsonFileName}");
            }
        }

        static string HtmlEncoded(string text)
        {
            string htmlDecoded = HttpUtility.HtmlDecode(text);
            return htmlDecoded != text ? htmlDecoded : text;
        }

        private IEnumerable<(string fileName, EmailRecord emailRecord)> DownloadMessages()
        {
            client.Folders.Inbox.Messages.Download("UNSEEN");

            LogMessage($"New Counter Emails {client.Folders.Inbox.Messages.Count(x => x.From.Address == arguments.FromAddress && !x.Seen)}");

            foreach (var message in client.Folders.Inbox.Messages.Where(x => x.From.Address == arguments.FromAddress && !x.Seen))
            {
                string messageSubject = message.Subject;
                string databaseName = messageSubject.StartsWith("Title Usage Report - ")
                    ? messageSubject.Replace("Title Usage Report - ", "").Replace(":", "꞉")
                    : throw new FormatException("Subject line in unknown format");

                MessageBody messageBody = message.Body;

                var doc = new HtmlDocument();
                doc.LoadHtml(messageBody.Html);

                HtmlNode reportTag = doc.DocumentNode.SelectNodes("//a[@href]").First();
                string reportLink = HtmlEncoded(reportTag.InnerHtml);

                if (string.IsNullOrWhiteSpace(reportLink) || reportLink == "http://eadmin.ebscohost.com/eadmin/login.aspx ")
                    continue;

                var regex = new Regex(@"\d{8}_\d{8}([.]xslx|[.]tsv)");
                string reportingPeriod = regex.Match(reportLink).ToString();
                var reportDate = DateTime.ParseExact(reportingPeriod.Substring(0, 8), "yyyyMMdd", null).AddMonths(1);

                var webClient = new WebClient();

                EmailRecord emailRecord = new EmailRecord(databaseName, webClient.DownloadString(reportLink), reportDate);

                string localEmailFileName = $"{Name} {emailRecord.Date:yyyy-MM-dd} {emailRecord.Database}.tsv";

                if (arguments.JsonRepository != null)
                {
                    using (IDirectoryRepository directoryRepo = RepositoryFactory.CreateDirectoryRepository(arguments.JsonRepository))
                    {
                        using (Stream stream = directoryRepo.CreateFile(localEmailFileName, FileCreationMode.Overwrite))
                        {
                            using (StreamWriter streamWriter = new StreamWriter(stream))
                            {
                                streamWriter.Write(emailRecord.Data);
                            }
                        }
                    }
                }

                message.Seen = true;

                yield return (localEmailFileName, emailRecord);
            }
        }

        private IEnumerable<CounterRecord> ParseMessages(IEnumerable<EmailRecord> messages)
        {
            void CheckForOptionalHeaders(string headers, DelimitedFileEngine<EbscoBookEmailRecord> delimitedFileEngine1, FieldInfo fieldInfo, FieldBase fieldBase)
            {
                if (headers.Contains(fieldInfo.Name.Replace("_", " ")))
                {
                    if (!delimitedFileEngine1.Options.FieldsNames.Contains(fieldInfo.Name))
                        delimitedFileEngine1.Options.Fields.Add(fieldBase);
                }
                else
                {
                    if (delimitedFileEngine1.Options.FieldsNames.Contains(fieldInfo.Name))
                    {
                        delimitedFileEngine1.Options.RemoveField(fieldInfo.Name);
                    }
                }
            }

            var delimitedFileEngine = new DelimitedFileEngine<EbscoBookEmailRecord> { Options = { IgnoreFirstLines = 8, IgnoreEmptyLines = true }, ErrorMode = ErrorMode.ThrowException };

            TypedRecordAttribute typedRecordAttribute = typeof(EbscoBookEmailRecord).GetCustomAttribute<TypedRecordAttribute>(true);

            FieldInfo bookFieldInfo = typeof(EbscoBookEmailRecord).GetField("Book_ID");
            FieldBase bookIdField = FieldBase.CreateField(bookFieldInfo, typedRecordAttribute);

            FieldInfo ebookfieldInfo = typeof(EbscoBookEmailRecord).GetField("eBook_Online_Requests");
            FieldBase ebookIdField = FieldBase.CreateField(ebookfieldInfo, typedRecordAttribute);

            var emailRecords = messages as EmailRecord[] ?? messages.ToArray();

            foreach (EmailRecord message in emailRecords)
            {
                yield return new CounterRecord
                {
                    ItemName = message.Database,
                    ItemType = ItemType.Database,
                    ItemPlatform = Name,
                    RunDate = message.Date,
                    Identifiers = new[]
                    {
                        new CounterIdentifier
                        {
                            IdentifierType = IdentifierType.Database,
                            IdentifierValue = Name
                        }
                    }, Metrics = new CounterMetric[] { }
                };

                yield return new CounterRecord { ItemName = Name, ItemType = ItemType.Vendor, RunDate = message.Date, Identifiers = new[] { new CounterIdentifier { IdentifierType = IdentifierType.Proprietary, IdentifierValue = Name } }, Metrics = new CounterMetric[] { } };

                string headers = message.Data.Substring(0, 1400 > message.Data.Length ? message.Data.Length : 1400);

                CheckForOptionalHeaders(headers, delimitedFileEngine, bookFieldInfo, bookIdField);
                CheckForOptionalHeaders(headers, delimitedFileEngine, ebookfieldInfo, ebookIdField);

                foreach (var record in delimitedFileEngine.ReadStringAsList(message.Data))
                {
                    yield return new CounterRecord
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
                    };
                }
            }
        }

        #region IRepository

        public override string ConnectionString => $"Email | Host = {arguments.HostName} Port = {arguments.Port} Username = {arguments.Username}";

        public IEnumerable<CounterReport> AvailableReports => new List<CounterReport> { CounterReport.JR1 };

        #endregion

        public override void Dispose()
        {
            client.Disconnect();
            client.Dispose();
        }

        private struct EmailRecord
        {
            public readonly string Database;
            public readonly string Data;
            public readonly DateTime Date;

            public EmailRecord(string database, string data, DateTime date)
            {
                Database = database;
                Data = data;
                Date = date;
            }
        }

        public Action<String> LogMessage { get; set; }
    }
}