using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Directory;

namespace ZondervanLibrary.Harvester.Core.Operations.Sync
{
    /// <summary>
    /// Encapsulates an operation that synchronizes the contents of one directory repository with another.
    /// </summary>
    public class SyncOperation : OperationBase
    {
        private readonly SyncOperationArguments arguments;
        private readonly RepositoryArgumentsBase sourceDirectory;
        private readonly RepositoryArgumentsBase destinationDirectory;

        public SyncOperation(SyncOperationArguments arguments, RepositoryArgumentsBase destinationDirectory, RepositoryArgumentsBase sourceDirectory)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(sourceDirectory != null);
            Contract.Requires(destinationDirectory != null);

            this.arguments = arguments;
            this.sourceDirectory = sourceDirectory;
            this.destinationDirectory = destinationDirectory;
        }

        public override void Execute(DateTime runDate, Action<string> logMessage, CancellationToken cancellationToken)
        {
            using (IDirectoryRepository source = RepositoryFactory.CreateDirectoryRepository(sourceDirectory))
            {
                logMessage($"Connected to source repository '{source.Name}' ({source.ConnectionString})");

                using (IDirectoryRepository destination = RepositoryFactory.CreateDirectoryRepository(destinationDirectory))
                {
                    logMessage($"Connected to destination repository '{destination.Name}' ({destination.ConnectionString})");

                    Regex filePattern = new Regex(arguments.FilePattern, RegexOptions.IgnoreCase);
                    List<DirectoryObjectMetadata> destinationFiles = destination.ListFiles("/").ToList();
                    int newCount = 0, modified = 0;
                    foreach (DirectoryObjectMetadata file in source.ListFiles("/").Where(x => filePattern.IsMatch(x.Name)))
                    {
                        try
                        {
                            if (!destinationFiles.Select(x => x.Name).Contains(file.Name))
                            {
                                logMessage($"Processing {file.Name} from {source.Name} to {destination.Name}");

                                using (Stream destStream = destination.CreateFile("TMP" + file.Name, FileCreationMode.Overwrite))
                                {
                                    using (Stream sourceStream = source.OpenFile(file.Name))
                                    {
                                        sourceStream.CopyTo(destStream);
                                    }
                                }

                                destination.CopyFile("TMP" + file.Name, file.Name);
                                destination.DeleteFile("TMP" + file.Name);

                                logMessage($"Added {file.Name} from {source.Name} to {destination.Name}");
                                newCount++;

                                if (cancellationToken.IsCancellationRequested)
                                {
                                    source.Dispose();
                                    destination.Dispose();
                                    cancellationToken.ThrowIfCancellationRequested();
                                }
                            }
                            else
                            {
                                DirectoryObjectMetadata destinationFile = destinationFiles.Find(x => x.Name == file.Name);
                                if (file.ModifiedDate > destinationFile.ModifiedDate)
                                {
                                    logMessage($"Processing {file.Name} from {source.Name} to {destination.Name}");

                                    using (Stream destStream = destination.CreateFile("TMP" + file.Name, FileCreationMode.Overwrite))
                                    {
                                        using (Stream sourceStream = source.OpenFile(file.Name))
                                        {
                                            sourceStream.CopyTo(destStream);
                                        }
                                    }

                                    destination.DeleteFile(file.Name);
                                    destination.MoveFile("TMP" + file.Name, file.Name);

                                    logMessage($"Updated {file.Name} from {source.Name} to {destination.Name}");
                                    modified++;

                                    if (cancellationToken.IsCancellationRequested)
                                    {
                                        source.Dispose();
                                        destination.Dispose();
                                        cancellationToken.ThrowIfCancellationRequested();
                                    }
                                }
                            }
                        }
                        catch (Exception)
                        {
                            destination.DeleteFile("TMP" + file.Name);
                            throw;
                        }
                    }

                    logMessage($"Discovered {newCount} new files and {modified} updated files).");
                }
            }
        }
    }
}
