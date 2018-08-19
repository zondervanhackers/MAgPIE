using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
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

        public override void Execute(DateTime runDate, Action<String> logMessage, CancellationToken cancellationToken)
        {
            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(_directoryArgs))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                List<String> modified = new List<String>();
                Int32 newCount = 0;

                using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(_harvesterArgs))
                {
                    logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                    Regex filePattern = new Regex(_arguments.FilePattern, RegexOptions.IgnoreCase);
                    IEnumerable<DirectoryObjectMetadata> sourceFiles = source.ListFiles().Where(x => filePattern.IsMatch(x.Name));
                    Dictionary<String, DirectoryRecord> dictionary = harvester.DataContext.DirectoryRecords.Where(d => d.Operation.Name == Name && d.Repository.Name == source.Name).ToDictionary(d => d.FilePath);

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

                            harvester.DataContext.DirectoryRecords.InsertOnSubmit(new DirectoryRecord
                            {
                                OperationID = OperationID,
                                RepositoryID = repository.ID,
                                FilePath = file.Path,
                                FileModifiedDate = file.ModifiedDate,
                                CreationDate = DateTime.Now,
                                ModifiedDate = DateTime.Now
                            });
                        }
                        else
                        {
                            DirectoryRecord element = dictionary[file.Path];

                            if (file.ModifiedDate > element.FileModifiedDate)
                            {
                                modified.Add(file.Path);
                                element.FileModifiedDate = file.ModifiedDate;
                                element.ModifiedDate = DateTime.Now;
                            }
                        }
                    }
                    if (cancellationToken.IsCancellationRequested)
                    {
                        source.Dispose();
                        harvester.Dispose();
                        cancellationToken.ThrowIfCancellationRequested();
                    }
                    harvester.DataContext.SubmitChanges();
                }

                logMessage($"Discovered {modified.Count} files to be processed ({newCount} new and {modified.Count - newCount} updated).");

                if (modified.Count == 0)
                    return;

                List<StatistaRecord> records = new List<StatistaRecord>();

                foreach (String file in modified)
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
                    Int32[] necessaryIndices = new[] { fieldMap["date"], fieldMap["title"], fieldMap["type of access"] };
                    foreach (List<String> line in lines)
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
                             Date = DateTime.Parse(line[fieldMap["date"]]),
                             ID = line[fieldMap["id"]],
                             ContentType = line[fieldMap["content type"]],
                             MainIndustry = line[fieldMap["main industry"]],
                             Title = line[fieldMap["title"]],
                             TypeofAccess = line[fieldMap["type of access"]],
                             Content = line[fieldMap["content"]],
                             Subtype = fieldMap.ContainsKey("subtyp") ? line[fieldMap["subtyp"]] : null,
                         });
                    }

                    logMessage($"\t{records.Count}/{records.Count + recordsSkipped} records processed.");
                }

                if (records.Count == 0)
                    return;

                using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(_statisticsArgs))
                {
                    destination.DataContext.BulkImportStatista(
                        records.ToDataReader(r => new object[] { r.ID, r.Date, r.ContentType, r.MainIndustry, r.Title, r.TypeofAccess, r.Content, r.Subtype }));
                }
            }
        }


        private static bool EmptyField(string cellText)
        {
            return cellText == "" || cellText == "-";
        }
    }
}