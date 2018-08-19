using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations.EZProxy
{
    public class ImportEZProxyLogOperation : OperationBase
    {
        private readonly RepositoryArgumentsBase _harvesterArgs;
        private readonly RepositoryArgumentsBase _directoryArgs;
        private readonly RepositoryArgumentsBase _statisticsArgs;

        private static readonly string legacyRegex = "(?<IP>^(?:\\d+\\.){3}\\d+) (?<username1>\\S+) (?<Username>\\S+) (?:\\[(?<DateTime>.*)\\]) \"(?:[A-Za-z]+):? (?<Request>.*)\\s?HTTP\\/\\d.\\d\\s?\" (?<HTTPCode>\\d+) (?<BytesTransferred>\\d+)";
        private static readonly string overallRegex = legacyRegex + " \"(?<Referer>http.+|-)\" \"(?<UserAgent>.+)\"$";

        private readonly Regex legacyExpression = new Regex(legacyRegex);
        private readonly Regex overallExpression = new Regex(overallRegex);
        private Action<string> LogMessage;

        public ImportEZProxyLogOperation(ImportEZProxyLogOperationArguments arguments, RepositoryArgumentsBase HarvesterDatabase, RepositoryArgumentsBase DestinationDatabase, RepositoryArgumentsBase SourceDirectory)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(SourceDirectory != null);
            Contract.Requires(HarvesterDatabase != null);
            Contract.Requires(DestinationDatabase != null);

            _directoryArgs = SourceDirectory;
            _harvesterArgs = HarvesterDatabase;
            _statisticsArgs = DestinationDatabase;
        }

        public override void Execute(DateTime runDate, Action<string> logMessage, System.Threading.CancellationToken cancellationToken)
        {
            LogMessage = logMessage;

            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(_directoryArgs))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                List<String> modified = new List<String>();
                Int32 newCount = 0;

                IEnumerable<DirectoryObjectMetadata> sourceFiles = source.ListFiles("/").ToList();

                using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(_harvesterArgs))
                {
                    logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                    if (OperationID == 0)
                    {
                        logMessage("Warning: OperationID was not set properly. Correcting this.");
                        OperationID = harvester.DataContext.Operations.First(d => d.Name == Name).ID;
                    }

                    Dictionary<String, DirectoryRecord> dictionary = harvester.DataContext.DirectoryRecords.Where(d => d.Operation.Name == Name && d.Repository.Name == source.Name).ToDictionary(d => d.FilePath);

                    foreach (DirectoryObjectMetadata file in sourceFiles)
                    {
                        if (!dictionary.ContainsKey(file.Path))
                        {
                            modified.Add(file.Name);
                            newCount++;
                        }
                        else
                        {
                            DirectoryRecord element = dictionary[file.Path];

                            if (file.ModifiedDate > element.FileModifiedDate)
                            {
                                modified.Add(file.Name);
                            }
                        }
                    }
                }

                if (modified.Count == 0)
                {
                    logMessage("No new files to process");
                    return;
                }

                logMessage($"Discovered {modified.Count} files to be processed ({newCount} new and {modified.Count - newCount} updated).");

                if (cancellationToken.IsCancellationRequested)
                {
                    source.Dispose();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(_statisticsArgs))
                {
                    logMessage($"Connected to database '{destination.Name}' ({destination.ConnectionString})");

                    var successfulFiles = new List<DirectoryObjectMetadata>();


                    var records = modified.Select(file =>
                    {
                        IEnumerable<EZProxyLog> logs = ParseFile(source, file);

                        successfulFiles.Add(sourceFiles.First(x => x.Name == file));

                        return Tuple.Create(logs, (Action) (() =>
                        {
                            logMessage($"Parsed file {file}");
                        }));

                    }).SelectMany(x => x);

                    try
                    {
                        destination.DataContext.BulkImportEZProxyLog(records.ToDataReader(r => new object[] { r.IP, r.Username, r.DateTime, r.Request, r.HTTPCode, r.BytesTransferred, r.Referer, r.UserAgent }));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        UpdateHarvesterRecord(LogMessage, successfulFiles, source.Name, _harvesterArgs);
                        throw;
                    }
                    finally
                    {
                        destination.DataContext.Connection.Close();
                        destination.DataContext.Dispose();
                    }

                    UpdateHarvesterRecord(LogMessage, successfulFiles, source.Name, _harvesterArgs);
                }
            }
        }

        private IEnumerable<EZProxyLog> ParseFile(IDirectoryRepository source, string file)
        {
            Stream stream = source.OpenFile(file);
            StreamReader reader = new StreamReader(stream);
            string line;
            int index = 0;

            while ((line = reader.ReadLine()) != null)
            {
                index++;
                bool overall = overallExpression.IsMatch(line);

                if (!overall && !legacyExpression.IsMatch(line))
                {
                    LogMessage($"  ERROR on Line {index} Unknown log format found: {line}");
                    continue;
                    //throw new InvalidDataException("ERROR Unknown log format found: " + line);
                }

                Match match = overall ? overallExpression.Match(line) : legacyExpression.Match(line);
                GroupCollection groups = match.Groups;

                if (overall)
                {
                    yield return new EZProxyLog
                    {
                        IP = groups["IP"].Value,
                        Username = ParseUsername(groups["Username"].Value),
                        DateTime = ParseDate(groups["DateTime"].Value),
                        Request = groups["Request"].Value,
                        HTTPCode = Int16.Parse(groups["HTTPCode"].Value),
                        BytesTransferred = Int32.Parse(groups["BytesTransferred"].Value),
                        Referer = ParseNull(groups["Referer"].Value),
                        UserAgent = ParseNull(groups["UserAgent"].Value),
                    };
                }
                else
                {
                    yield return new EZProxyLog
                    {
                        IP = groups["IP"].Value,
                        Username = ParseUsername(groups["Username"].Value),
                        DateTime = ParseDate(groups["DateTime"].Value),
                        Request = groups["Request"].Value,
                        HTTPCode = Int16.Parse(groups["HTTPCode"].Value),
                        BytesTransferred = Int32.Parse(groups["BytesTransferred"].Value),
                    };
                }
            }
        }

        private static string ParseNull(string p)
        {
            return p == "-" ? null : p;
        }

        private static string ParseUsername(string username)
        {
            string tempUserName = username.Replace(" [", "");
            return tempUserName == "-" ? null : tempUserName;
        }

        private DateTime ParseDate(string date)
        {
            //01/Nov
            string[] dateParts = date.Split('/');
            int day = int.Parse(dateParts[0]);
            int month = ParseMonth(dateParts[1]);
            

            //2016:06:46:35 -400
            string[] dateEndParts = dateParts[2].Split(':', '-', '+');
            int year = int.Parse(dateEndParts[0]);
            int hour = int.Parse(dateEndParts[1]);
            int minutes = int.Parse(dateEndParts[2]);
            int seconds = int.Parse(dateEndParts[3]);

            double timezone = ParseTimeZone(dateEndParts[4], dateParts[2].First(x => x == '-' || x == '+'));

            DateTime parsed = new DateTime(year, month, day, hour, minutes, seconds);
            DateTime utcParsed = parsed.AddHours(timezone);

            return utcParsed;
        }

        private double ParseTimeZone(string timezone, char mod)
        {
            if (timezone.Substring(2) != "00")
                return double.Parse(mod + timezone.Substring(0, 2) + ":" + timezone.Substring(2));

            return double.Parse(mod + timezone.Substring(0, 2));
        }

        private static int ParseMonth(string month)
        {
            switch (month)
            {
                case "Jan":
                    return 1;
                case "Feb":
                    return 2;
                case "Mar":
                    return 3;
                case "Apr":
                    return 4;
                case "May":
                    return 5;
                case "Jun":
                    return 6;
                case "Jul":
                    return 7;
                case "Aug":
                    return 8;
                case "Sep":
                    return 9;
                case "Oct":
                    return 10;
                case "Nov":
                    return 11;
                case "Dec":
                    return 12;
                default:
                    throw new ArgumentException($"{month} is not a recognizable month.");
            }
        }
    }
}
