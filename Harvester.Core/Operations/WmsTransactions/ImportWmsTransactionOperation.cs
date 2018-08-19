using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.SharedLibrary.Pipeline;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsTransactions
{
    public class ImportWmsTransactionOperation : OperationBase
    {
        private readonly RepositoryArgumentsBase _databaseArgs;
        private readonly RepositoryArgumentsBase _harvesterArgs;
        private readonly RepositoryArgumentsBase _directoryArgs;

        public ImportWmsTransactionOperation(RepositoryArgumentsBase HarvesterDatabase, RepositoryArgumentsBase DestinationDatabase, RepositoryArgumentsBase SourceDirectory)
        {
            Contract.Requires(DestinationDatabase != null);
            Contract.Requires(SourceDirectory != null);
            Contract.Requires(HarvesterDatabase != null);

            _databaseArgs = DestinationDatabase;
            _harvesterArgs = HarvesterDatabase;
            _directoryArgs = SourceDirectory;
        }

        public override void Execute(DateTime runDate, Action<string> logMessage, CancellationToken cancellationToken)
        {
            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(_directoryArgs))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                List<String> modified = new List<String>();
                Int32 newCount = 0;

                List<DirectoryObjectMetadata> sourceFiles = source.ListFiles("/").Where(y => y.Name.ToLower().StartsWith("daily_transaction_export_for_vamp") && (y.Name.Contains(".txt") || y.Name.Contains(".csv"))).ToList();

                using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(_harvesterArgs))
                {
                    logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                    if (OperationID == 0)
                    {
                        logMessage("Warning: OperationID was not set properly. Correcting this.");
                        OperationID = harvester.DataContext.Operations.First(d => d.Name == Name).ID;
                    }
                    Dictionary<String, DirectoryRecord> dictionary = harvester.DataContext.DirectoryRecords.Where(d => d.OperationID == OperationID && d.RepositoryID == harvester.DataContext.Repositories.First(y => y.Name == source.Name).ID).ToDictionary(d => d.FilePath);

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

                logMessage($"Discovered {modified.Count} files to be processed ({newCount} new and {modified.Count - newCount} updated).");

                if (modified.Count == 0 && newCount == 0)
                {
                    logMessage("No Records to be processed.");
                    return;
                }

                if (cancellationToken.IsCancellationRequested)
                {
                    source.Dispose();
                    cancellationToken.ThrowIfCancellationRequested();
                }

                using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(_databaseArgs))
                {
                    logMessage($"Connected to destination database '{destination.Name}' ({destination.ConnectionString})");

                    var records = modified.Select(file =>
                    {
                        logMessage($"Processing '{file}':");

                        Stream inputStream = source.OpenFile(file);

                        CountingEnumerable<WmsTransactionRecord> counter = new CountingEnumerable<WmsTransactionRecord>(ReadLines(inputStream, GetFileDate(file)));

                        return Tuple.Create(counter, (Action) (() =>
                        {
                            inputStream.Dispose();
                            logMessage($"\t{counter.Count} records processed.");
                        }));

                    }).SelectMany(i => i).Select(wms => new
                    {
                        wms.EventType,
                        wms.InstitutionName,
                        ItemBarcode = wms.ItemBarcode == "N/A" ? null : wms.ItemBarcode,
                        UserBarcode = wms.UserBarcode == "N/A" ? null : wms.UserBarcode,
                        wms.LoanCheckedOutDate,
                        wms.LoanDueDate,
                        wms.RecordDate
                    }).ToList();

                    logMessage($"Processed {records.Count} records");

                    try
                    {
                        destination.DataContext.BulkImportTransactions(records.ToDataReader(x => new object[] {x.ItemBarcode, x.UserBarcode, x.LoanDueDate, x.LoanCheckedOutDate, runDate, x.EventType, x.InstitutionName, x.RecordDate}));
                        UpdateHarvesterRecord(logMessage, sourceFiles, source.Name, _harvesterArgs);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                    finally
                    {
                        destination.DataContext.Connection.Close();
                        destination.DataContext.Dispose();
                    }
                }
            }
        }

        private IEnumerable<WmsTransactionRecord> ReadLines(Stream inStream, DateTime fileDate)
        {
            string[] Fields = {"Item Barcode", "User Barcode", "Event Due Date", "Event Date", "Event Type", "Institution Name"};
            using (StreamReader reader = new StreamReader(inStream))
            {
                string line;
                reader.ReadLine();

                while ((line = reader.ReadLine()) != null)
                {
                    Dictionary<String, String> values = line.Split(new[] {"\t"}, StringSplitOptions.None).Select((x, i) => new {value = x, index = i}).ToDictionary(x => Fields[x.index], x => x.value.Replace("\"", ""));
                    if (values["User Barcode"].Length > 16)
                    {
                        string userBarcode = values["User Barcode"];
                        if (new Regex(@"^[A-Za-z]+\d+$").IsMatch(userBarcode))
                        {
                            char[] characterArray = {'0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};
                            int endOfCharactersInString = userBarcode.IndexOfAny(characterArray);
                            int stringToCutOut = userBarcode.Length - 16;

                            if (stringToCutOut <= endOfCharactersInString)
                                values["User Barcode"] = userBarcode.Remove(endOfCharactersInString - stringToCutOut, stringToCutOut);
                            else
                                throw new InvalidDataException($"UserBarcode ({userBarcode}) is too long and of no recognizable pattern to be shortened.");
                        }
                        else
                        {
                            throw new InvalidDataException($"UserBarcode ({userBarcode}) is too long and of no recognizable pattern to be shortened.");
                        }
                    }
                    WmsTransactionRecord record = new WmsTransactionRecord
                    {
                        EventType = values["Event Type"],
                        UserBarcode = values["User Barcode"],
                        ItemBarcode = values["Item Barcode"],
                        LoanCheckedOutDate = DateTime.Parse(values["Event Date"]),
                        LoanDueDate = DateTime.Parse(values["Event Due Date"]),
                        InstitutionName = values["Institution Name"],
                        RecordDate = fileDate
                    };
                    yield return record;
                }
            }
        }

        private DateTime GetFileDate(string filename)
        {
            string dateString = filename.Replace("Daily_Transaction_Export_for_VAMP.", "").Replace(".csv", "").Replace(".txt", "").Replace("Daily_Transaction_Export_for_VAMP", "");
            return DateTime.ParseExact(dateString, new[] {"yyyy-MM-dd", "yyyy-MM-dd-HH-mm-ss"}, null, System.Globalization.DateTimeStyles.None);
        }
    }
}
