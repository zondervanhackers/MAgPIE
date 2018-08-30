using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using OfficeOpenXml;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations.Statista
{
    public class ImportStatistaOperation : OperationBase
    {
        private readonly ImportStatistaOperationArguments _arguments;
        private readonly RepositoryArgumentsBase _harvesterArgs;
        private readonly RepositoryArgumentsBase _directoryArgs;
        private readonly RepositoryArgumentsBase _statisticsArgs;

        public ImportStatistaOperation(ImportStatistaOperationArguments arguments, RepositoryArgumentsBase HarvesterDatabase, RepositoryArgumentsBase DestinationDatabase, RepositoryArgumentsBase SourceDirectory)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(SourceDirectory != null);
            Contract.Requires(HarvesterDatabase != null);
            Contract.Requires(DestinationDatabase != null);

            _arguments = arguments;
            _directoryArgs = SourceDirectory;
            _harvesterArgs = HarvesterDatabase;
            _statisticsArgs = DestinationDatabase;
        }

        public override void Execute(DateTime runDate, Action<string> logMessage, CancellationToken cancellationToken)
        {
            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(_directoryArgs))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                List<string> modified = new List<string>();
                int newCount = 0;

                using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(_harvesterArgs))
                {
                    logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                    Regex filePattern = new Regex(_arguments.FilePattern, RegexOptions.IgnoreCase);
                    IEnumerable<DirectoryObjectMetadata> sourceFiles = source.ListFiles().Where(x => filePattern.IsMatch(x.Name)).ToArray();
                    Dictionary<string, DirectoryRecord> dictionary = harvester.DataContext.DirectoryRecords.Where(d => d.Operation.Name == Name && d.Repository.Name == source.Name).ToDictionary(d => d.FilePath);

                    Entities.Repository repository = harvester.DataContext.Repositories.First(x => x.Name == source.Name);

                    if (OperationID == 0)
                    {
                        logMessage("Warning: OperationID was not set properly. Correcting this.");
                        OperationID = harvester.DataContext.Operations.First(d => d.Name == Name).ID;
                    }

                    foreach (DirectoryObjectMetadata file in sourceFiles)
                    {
                        if (!dictionary.ContainsKey(file.Path))
                        {
                            modified.Add(file.Path);
                            newCount++;
                        }
                        else
                        {
                            DirectoryRecord element = dictionary[file.Path];

                            if (file.ModifiedDate > element.FileModifiedDate)
                            {
                                modified.Add(file.Path);
                            }
                        }
                    }

                    if (cancellationToken.IsCancellationRequested)
                    {
                        source.Dispose();
                        harvester.Dispose();
                        cancellationToken.ThrowIfCancellationRequested();
                    }


                    logMessage($"Discovered {modified.Count} files to be processed ({newCount} new and {modified.Count - newCount} updated).");

                    if (modified.Count == 0)
                        return;

                    List<StatistaRecord> records = new List<StatistaRecord>();

                    foreach (string file in modified)
                    {
                        logMessage($"Processing Statista file: {file}");

                        ExcelPackage package = new ExcelPackage(new FileInfo(file));
                        ExcelWorksheet worksheet = package.Workbook.Worksheets[1];
                        ExcelRange cells = worksheet.Cells;

                        List<string> Fields = new List<string>();

                        for (int k = 1; k <= worksheet.Dimension.Columns; k++)
                        {
                            Fields.Add(cells[1, k].Value.ToString());
                        }

                        Dictionary<string, int> fieldMap = Fields.Select((x, i) => new { index = i, value = x }).ToDictionary(x => x.value.ToLower(), x => x.index);

                        List<List<string>> lines = new List<List<string>>();
                        for (int i = 2; i <= worksheet.Dimension.Rows; i++)
                        {
                            List<string> line = new List<string>();
                            for (int j = 1; j <= worksheet.Dimension.Columns; j++)
                            {
                                line.Add(cells[i, j].Text);
                            }

                            lines.Add(line);
                        }

                        int recordsSkipped = 0;
                        int[] necessaryIndices = new[] { fieldMap["date"], fieldMap["title"], fieldMap["type of access"] };
                        foreach (List<string> line in lines)
                        {
                            if (line.All(EmptyField) || line.Where((x, i) => necessaryIndices.Contains(i)).Any(EmptyField))
                            {
                                recordsSkipped++;
                                continue;
                            }

                            for (int i = 0; i < line.Count; i++)
                            {
                                if (EmptyField(line[i]))
                                {
                                    line[i] = null;
                                }
                            }

                            records.Add(new StatistaRecord
                            {
                                Date = ParseDate(line[fieldMap["date"]]),
                                Title = line[fieldMap["title"]],
                                TypeofAccess = line[fieldMap["type of access"]],

                                ID = fieldMap.ContainsKey("id") ? line[fieldMap["id"]] : null,
                                ContentType = fieldMap.ContainsKey("content type") ? line[fieldMap["content type"]] : null,
                                MainIndustry = fieldMap.ContainsKey("main industry") ? line[fieldMap["main industry"]] : null,
                                Content = fieldMap.ContainsKey("content") ? line[fieldMap["content"]] : null,
                                Subtype = fieldMap.ContainsKey("subtyp") ? line[fieldMap["subtyp"]] : null,
                            });
                        }

                        logMessage($"\t{records.Count}/{records.Count + recordsSkipped} records processed.");
                    }

                    using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(_statisticsArgs))
                    {
                        destination.DataContext.BulkImportStatista(
                            records.ToDataReader(r => new object[] { r.ID, r.Date, r.ContentType, r.MainIndustry, r.Title, r.TypeofAccess, r.Content, r.Subtype }));
                    }

                    UpdateHarvesterRecord(logMessage, sourceFiles, source.Name, _harvesterArgs);

                }
            }
        }

        private static bool EmptyField(string cellText)
        {
            return cellText == "" || cellText == "-";
        }

        private static DateTime ParseDate(string datetimeString)
        {
            if (DateTime.TryParse(datetimeString, out DateTime result))
            {
                return result;
            }

            if (DateTime.TryParseExact(datetimeString, "dd/MM/yyyy HH:mm:ss", null, DateTimeStyles.None, out result))
            {
                return result;
            }

            throw new FormatException($"'{datetimeString}' is not a recognizable DateTime format.");
        }
    }
}