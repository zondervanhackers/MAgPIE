using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Binding;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations.Counter
{
    public class ImportCounterTransactionsOperation : OperationBase
    {
        private readonly RepositoryArgumentsBase counterRepoArguments;
        private readonly RepositoryArgumentsBase statisticsArguments;
        private readonly RepositoryArgumentsBase harvesterArguments;
        private readonly RepositoryArgumentsBase localJsonArguments;
        private int recordswithNoIdentifiers;
        private static readonly Dictionary<IdentifierType, string[]> IdentifierToIgnore = new Dictionary<IdentifierType, string[]>
        {
            { IdentifierType.Proprietary, new[] { "ProQuest:string", "ProQuest:String" } },
            { IdentifierType.Doi, new[] { "String" } }
        };

        public ImportCounterTransactionsOperation(RepositoryArgumentsBase destinationDatabase, RepositoryArgumentsBase harvesterDatabase, RepositoryArgumentsBase sourceCounter, RepositoryArgumentsBase localJsonArgs)
        {
            Contract.Requires(sourceCounter != null);
            Contract.Requires(destinationDatabase != null);

            statisticsArguments = destinationDatabase;
            harvesterArguments = harvesterDatabase;
            counterRepoArguments = sourceCounter;
            localJsonArguments = localJsonArgs;
        }

        /// <inheritdoc />
        public override void Execute(DateTime runDate, Action<string> logMessage, CancellationToken cancellationToken)
        {
            using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(statisticsArguments))
            {
                destination.DataContext.LogMessage = logMessage;

                logMessage($"Connected to destination database '{destination.Name}' ({destination.ConnectionString})");

                using (ICounterRepository source = RepositoryFactory.CreateCounterRepository(counterRepoArguments))
                {
                    source.LogMessage = logMessage;
                    logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                    Dictionary<CounterReport, List<CounterRecord>> preRecordsReports = source.AvailableReports.ToDictionary(report => report, report => source.RequestRecords(runDate, report).ToList());

                    if (!preRecordsReports.Any(x => x.Value.Count > 0))
                    {
                        logMessage("No Records for this date");
                        return;
                    }

                    if (localJsonArguments != null)
                    {
                        foreach (var reportRecord in preRecordsReports)
                        {
                            using (IDirectoryRepository directoryRepo = RepositoryFactory.CreateDirectoryRepository(localJsonArguments))
                            {
                                using (Stream stream = directoryRepo.CreateFile($"{source.Name} {runDate:yyyy-MM-dd} {reportRecord.Key}.json", FileCreationMode.Overwrite))
                                {
                                    JsonSerializer serializer = JsonSerializer.Create();
                                    using (JsonTextWriter jsonTextWriter = new JsonTextWriter(new StreamWriter(stream)))
                                    {
                                        serializer.Serialize(jsonTextWriter, reportRecord.Value);
                                    }
                                }
                            }
                        }
                    }

                    var preRecords = preRecordsReports.SelectMany(x => x.Value).ToList();

                    //Add Metrics of vendorRecords to single vendor record
                    foreach (var recordsByRunDate in preRecords.GroupBy(x => x.RunDate))
                    {
                        if (recordsByRunDate.Any(x => x.ItemType == ItemType.Vendor))
                        {
                            foreach (var vendorRecord in recordsByRunDate.Where(x => x.ItemType == ItemType.Vendor))
                            {
                                vendorRecord.Identifiers = new[] { new CounterIdentifier { IdentifierType = IdentifierType.Proprietary, IdentifierValue = source.Name } };
                            }
                        }
                        else
                        {
                            preRecords.Add(new CounterRecord { ItemName = source.Name, ItemType = ItemType.Vendor, RunDate = recordsByRunDate.Select(y => y.RunDate).First(), Identifiers = new[] { new CounterIdentifier { IdentifierType = IdentifierType.Proprietary, IdentifierValue = source.Name } }, Metrics = new CounterMetric[] { } });
                        }
                    }

                    if (preRecords.Any(x => x.ItemType == ItemType.Database))
                    {
                        foreach (CounterRecord record in preRecords)
                        {
                            if (record.ItemType != ItemType.Database)
                                continue;

                            record.ItemPlatform = source.Name;
                            record.Identifiers = new[] { new CounterIdentifier { IdentifierType = IdentifierType.Database, IdentifierValue = record.ItemName } };
                        }
                    }
                    else
                    {
                        preRecords.Add(new CounterRecord { ItemName = source.Name, ItemType = ItemType.Database, ItemPlatform = source.Name, RunDate = runDate, Identifiers = new[] { new CounterIdentifier { IdentifierType = IdentifierType.Database, IdentifierValue = source.Name } }, Metrics = new CounterMetric[] { } });
                    }

                    var where = preRecords.Where(x => x.ItemType == ItemType.Vendor);

                    Console.WriteLine(where.Count());

                    var records = preRecords
                        .Where(x => !string.IsNullOrEmpty(x.ItemName))
                        .Select(SanitizeIdentifiers)
                        .GroupBy(x => x, new CounterRecordComparer())
                        .Select(AggregateDuplicates)
                        .Select((r, i) => new { Index = i, Record = r });

                    var counterRecords = records.GroupBy(x => x.Record.RunDate).ToList();

                    logMessage($"{preRecords.Count + 1} Total Records");
                    logMessage($"{recordswithNoIdentifiers} Records with no Identifiers");
                    logMessage($"{counterRecords.Sum(x => x.Count())} Unique Records");

                    if (cancellationToken.IsCancellationRequested)
                    {
                        source.Dispose();
                        destination.DataContext.Connection.Close();
                        destination.DataContext.Dispose();
                        destination.Dispose();
                        cancellationToken.ThrowIfCancellationRequested();
                    }

                    foreach (var counterGroup in counterRecords)
                    {
                        destination.DataContext.BulkImportCounterTransactions(
                            counterGroup
                                .ToDataReader(r => new object[] { null, null, null, r.Index, r.Record.ItemName, r.Record.ItemPlatform, r.Record.ItemType, r.Record.RunDate }),
                            counterGroup
                                .SelectMany(r => r.Record.Identifiers.Bind(a => a.Select(i => new { r.Index, Identifier = i, r.Record.ItemType })))
                                .ToDataReader(i => new object[] { i.Index, i.Identifier.IdentifierType, i.Identifier.IdentifierValue, i.ItemType }),
                            counterGroup
                                .SelectMany(r => r.Record.Metrics.Bind(a => a.Select(m => new { r.Index, Metric = m })))
                                .ToDataReader(m => new object[] { m.Index, m.Metric.MetricType, m.Metric.MetricValue }));

                        //Add Record of report being run
                        using (var harvester = RepositoryFactory.CreateHarvesterRepository(harvesterArguments))
                        {
                            if (OperationID == 0)
                            {
                                logMessage("Warning: OperationID was not set properly. Correcting this.");
                                OperationID = harvester.DataContext.Operations.First(d => d.Name == Name).ID;
                            }

                            Entities.Repository repository = harvester.DataContext.Repositories.First(x => x.Name == source.Name);

                            foreach (CounterReport report in preRecordsReports.Keys)
                            {
                                if (!harvester.DataContext.CounterOperationRecords.Any(x => x.OperationID == OperationID && x.RunDate == runDate && x.Report == report.ToString()))
                                {
                                    harvester.DataContext.CounterOperationRecords.InsertOnSubmit(new CounterOperationRecord
                                    {
                                        OperationID = OperationID,
                                        RepositoryID = repository.ID,
                                        RunDate = runDate,
                                        Report = report.ToString(),
                                        ExecutedDate = DateTime.Now
                                    });
                                }
                                else
                                {
                                    harvester.DataContext.CounterOperationRecords.First(x => x.OperationID == OperationID && x.RunDate == runDate && x.Report == report.ToString()).ExecutedDate = DateTime.Now;
                                }
                            }

                            harvester.DataContext.SubmitChanges();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Normalize Database Records
        /// </summary>
        /// <param name="preRecords"></param>
        /// <param name="sourceName"></param>
        private static void FixExistingDatabaseRecords(List<CounterRecord> preRecords, string sourceName)
        {
            foreach (CounterRecord record in preRecords)
            {
                if (record.ItemType != ItemType.Database)
                    continue;

                record.ItemPlatform = sourceName;
                record.Identifiers = new[] {new CounterIdentifier {IdentifierType = IdentifierType.Database, IdentifierValue = record.ItemName}};
            }
        }

        private CounterRecord SanitizeIdentifiers(CounterRecord x)
        {
            x.Identifiers = RemoveDuplicateIdentifiers(x);
            x.Identifiers = AddIdentifierToRecordIfDoesntExist(x);

            x.Metrics = x.Metrics.Where(y => y != null).ToArray();
            return x;
        }

        /// <summary>
        /// Remove known bad identifiers
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        private static bool BadIdentifier(CounterIdentifier x)
        {
            return IdentifierToIgnore.ContainsKey(x.IdentifierType) && IdentifierToIgnore[x.IdentifierType].Any(y => y == x.IdentifierValue) || string.IsNullOrWhiteSpace(x.IdentifierValue);
        }

        private static CounterRecord AggregateDuplicates<T>(IGrouping<T, CounterRecord> records)
        {
            if (records.Count() <= 1) return records.First();

            List<CounterRecord> listRecords = records.ToList();
            if (records.Count() <= 1)
                return records.First();

            CounterRecord record = new CounterRecord
            {
                ItemName = listRecords[0].ItemName,
                ItemPlatform = listRecords[0].ItemPlatform,
                ItemType = listRecords[0].ItemType,
                RunDate = listRecords[0].RunDate,
                Identifiers = listRecords.SelectMany(x => x.Identifiers).Distinct(new CounterIdentifierComparer()).ToArray(),
                Metrics = listRecords.SelectMany(x => x.Metrics).Distinct(new CounterMetricComparer()).GroupBy(x => x.MetricType).Select(x => new CounterMetric {MetricType = x.Key, MetricValue = x.Sum(y => y.MetricValue)}).ToArray(),
            };

            return record;
        }

        private static string CreateUniqueHash(string name)
        {
            return string.Join("", name.Substring(0, 1), name.Length, name.GetHashCode().ToString(), name.Substring(name.Length - 1, 1));
        }

        private CounterIdentifier[] AddIdentifierToRecordIfDoesntExist(CounterRecord record)
        {
            while (true)
            {
                if (record.Identifiers == null || record.Identifiers.Length == 0)
                {
                    if (record.ItemType == ItemType.Database)
                        return record.Identifiers = new[] {new CounterIdentifier {IdentifierType = IdentifierType.Database, IdentifierValue = record.ItemName}};

                    if (record.ItemName == "")
                    {
                        if (record.ItemPlatform == "")
                            throw new Exceptions.OperationException("Record Contained no Identifiers and no Identifiable Names");

                        record.ItemName = record.ItemPlatform;
                    }

                    recordswithNoIdentifiers++;
                    //Gives Records with No Identifier a Non-Identifier with a uniquely generated Value
                    return record.Identifiers = new[] {new CounterIdentifier {IdentifierType = IdentifierType.NoIdentifier, IdentifierValue = CreateUniqueHash(record.ItemName)}};
                }


                CounterIdentifier[] nonNullIdentifiers = record.Identifiers.Where(x => x.IdentifierValue != null).ToArray();
                if (nonNullIdentifiers.Length == 0)
                {
                    record.Identifiers = null;
                }
                else
                {
                    return nonNullIdentifiers;
                }
            }
        }

        private static CounterIdentifier[] RemoveDuplicateIdentifiers(CounterRecord record)
        {
            return record.Identifiers.Where(x => !BadIdentifier(x)).Distinct(new CounterIdentifierComparer()).ToArray();
        }

        private class CounterIdentifierComparer : IEqualityComparer<CounterIdentifier>
        {            
            public bool Equals(CounterIdentifier a, CounterIdentifier b)
            {
                return a != null && b != null && a.IdentifierType == b.IdentifierType && a.IdentifierValue == b.IdentifierValue;
            }

            public int GetHashCode(CounterIdentifier a)
            {
                return (a.IdentifierType + a.IdentifierValue).GetHashCode();
            }
        }

        private class CounterMetricComparer : IEqualityComparer<CounterMetric>
        {
            public bool Equals(CounterMetric a, CounterMetric b)
            {
                return a != null && b != null && a.MetricType == b.MetricType && a.MetricValue == b.MetricValue;
            }

            public int GetHashCode(CounterMetric a)
            {
                return (a.MetricType + a.MetricValue).GetHashCode();
            }
        }

        private class CounterRecordComparer : IEqualityComparer<CounterRecord>
        {
            public bool Equals(CounterRecord a, CounterRecord b)
            {
                Debug.Assert(a != null, nameof(a) + " != null");
                Debug.Assert(b != null, nameof(b) + " != null");

                if (a.ItemPlatform != b.ItemPlatform || a.ItemType != b.ItemType || a.RunDate != b.RunDate)
                    return false;

                var commonIdentifiers = a.Identifiers.Intersect(b.Identifiers, new CounterIdentifierComparer());

                if (commonIdentifiers.Any())
                    return true;

                if (string.Equals(a.ItemName, b.ItemName, StringComparison.CurrentCultureIgnoreCase))
                    return true;

                if (a.Identifiers.Length != b.Identifiers.Length)
                    return false;

                return false;
            }

            public int GetHashCode(CounterRecord r)
            {
                return (int)r.ItemType;
            }
        }
    }
}