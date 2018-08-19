using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Entities;
using ZondervanLibrary.SharedLibrary.Collections;
using ZondervanLibrary.SharedLibrary.Parsing;
using ZondervanLibrary.Statistics.Entities;

namespace ZondervanLibrary.Harvester.Core.Operations.EZProxy
{
    public class ImportEZProxyAuditOperation : OperationBase
    {
        private readonly ImportEZProxyAuditOperationArguments _arguments;
        private readonly RepositoryArgumentsBase _harvesterArgs;
        private readonly RepositoryArgumentsBase _directoryArgs;
        private readonly RepositoryArgumentsBase _statisticsArgs;
        private readonly RepositoryArgumentsBase _logDirectoryArgs;

        public ImportEZProxyAuditOperation(ImportEZProxyAuditOperationArguments arguments, RepositoryArgumentsBase HarvesterDatabase, RepositoryArgumentsBase DestinationDatabase, RepositoryArgumentsBase SourceDirectory, RepositoryArgumentsBase LogDirectory)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(SourceDirectory != null);
            Contract.Requires(HarvesterDatabase != null);
            Contract.Requires(DestinationDatabase != null);
            Contract.Requires(LogDirectory != null);

            _arguments = arguments;
            _directoryArgs = SourceDirectory;
            _harvesterArgs = HarvesterDatabase;
            _statisticsArgs = DestinationDatabase;
            _logDirectoryArgs = LogDirectory;
        }

        public override void Execute(DateTime runDate, Action<string> logMessage, System.Threading.CancellationToken cancellationToken)
        {
            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(_directoryArgs))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                using (IDirectoryRepository destination = RepositoryFactory.CreateDirectoryRepository(_logDirectoryArgs))
                {
                    logMessage($"Connected to destination repository '{destination.Name}' ({destination.ConnectionString})");

                    Regex filePattern = new Regex(_arguments.FilePattern);

                    foreach (DirectoryObjectMetadata file in source.ListFiles().Where(x => x.Path.Contains(".zip")))
                    {
                        string tempZipDirectoryPath = file.Path.Replace(".zip", "");
                        string tempZipDirectoryName = file.Name.Replace(".zip", "");
                        ZipFile.ExtractToDirectory(file.Path, tempZipDirectoryPath);
                        source.DeleteFile(file.Path);

                        foreach (String unzippedfile in source.ListFiles(tempZipDirectoryName).Select(x => x.Path).Where(x => filePattern.IsMatch(x)))
                        {
                            string filename = unzippedfile.Split(new[] { "\\" }, StringSplitOptions.None).Last();
                            List<String> currentFiles = source.ListFiles().Select(x => x.Name).ToList();
                            if (!currentFiles.Contains(filename))
                            {
                                source.MoveFile(unzippedfile, filename);
                            }
                        }

                        foreach (String gzipFile in source.ListFiles(tempZipDirectoryName).Select(x => x.Path).Where(x => x.Contains(".gz")))
                        {
                            string fileNameConcat = tempZipDirectoryName + ".log";
                            List<String> currentFiles = destination.ListFiles().Select(x => x.Name).ToList();
                            if (!currentFiles.Contains(fileNameConcat))
                            {
                                using (GZipStream gzipStream = new GZipStream(source.OpenFile(gzipFile), CompressionMode.Decompress))
                                {
                                    using (Stream unzippedDestination = destination.CreateFile(fileNameConcat, Repository.Directory.FileCreationMode.ThrowIfFileExists))
                                    {
                                        gzipStream.CopyTo(unzippedDestination);
                                    }
                                }
                            }
                        }

                        source.DeleteDirectory(tempZipDirectoryPath);
                    }
                }

                List<String> modified = new List<String>();
                Int32 newCount = 0;

                using (IDatabaseRepository<IHarvesterDataContext> harvester = RepositoryFactory.CreateHarvesterRepository(_harvesterArgs))
                {
                    logMessage($"Connected to database '{harvester.Name}' ({harvester.ConnectionString})");

                    IEnumerable<DirectoryObjectMetadata> sourceFiles = source.ListFiles("/");
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
                            modified.Add(file.Name);
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
                                modified.Add(file.Name);
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

                using (IDatabaseRepository<IStatisticsDataContext> destination = RepositoryFactory.CreateStatisticsRepository(_statisticsArgs))
                {
                    logMessage($"Connected to database '{destination.Name}' ({destination.ConnectionString})");

                    StreamParser<EZProxyAudit> Parser = new StreamParser<EZProxyAudit>();

                    List<EZProxyAudit> records = modified.Select(file =>
                    {
                        logMessage($"Processing '{file}':");

                        int lineNumber = 0;

                        return Parser.ParseStream(source.OpenFile(file)).Select(x => new EZProxyAudit
                        {
                            DateTime = x.DateTime,
                            Event = x.Event,
                            IP = x.IP,
                            Other = x.Other,
                            Session = x.Session,
                            Username = x.Username,
                            LineNumber = lineNumber++, 
                        });

                    }).SelectMany(x => x).ToList();

                    logMessage($"Records Found: {records.Count}");

                    destination.DataContext.BulkImportEZProxyAudit(records.ToDataReader(r => new object[] { r.DateTime, r.Event, r.IP, r.Username, r.Session, r.Other, r.LineNumber }));

                }
            }
        }
    }
}
