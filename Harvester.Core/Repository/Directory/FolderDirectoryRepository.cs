using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.IO;
using System.Net.NetworkInformation;
using System.Threading;
using ZondervanLibrary.Harvester.Core.Exceptions;
using ZondervanLibrary.Harvester.Core.Permissions;

namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    public class FolderDirectoryRepository : RepositoryBase, IDirectoryRepository
    {
        private readonly FolderDirectoryRepositoryArguments _arguments;

        public FolderDirectoryRepository(FolderDirectoryRepositoryArguments arguments)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(arguments.Path != null);
            Contract.Requires(arguments.Name != null);

            Name = arguments.Name;
            _arguments = arguments;
            RepositoryId = new Guid();

            if (!System.IO.Directory.Exists(_arguments.Path))
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.DirecoryNotFound, this, String.Format(RepositoryExceptionMessage.DirectoryNotFound_1, _arguments.Path));
            }
        }

        private FolderDirectoryRepositoryArguments Arguments => _arguments;

        private string GetFilePath(string fileName)
        {
            try
            {
                return Path.Combine(_arguments.Path, fileName);
            }
            catch (ArgumentException exception)
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidHost, this, String.Format(RepositoryExceptionMessage.InvalidCharacter_1, fileName), exception);
            }
        }

        /// <summary>
        /// Handles exceptions common to <see cref="CreateFile"/>, <see cref="DeleteFile"/>, <see cref="MoveFile"/>, and <see cref="OpenFile"/>.
        /// </summary>
        private Stream HandleException(Exception exception, string filePath, string fileName)
        {
            try
            {
                throw exception;
            }
            catch (ArgumentException)
            {
                // Thrown if filePath is invalid.
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNotFound, this, String.Format(RepositoryExceptionMessage.InvalidFormat_1, fileName), exception);
            }
            catch (PathTooLongException)
            {
                // Thrown if part of the fileName exceeds the system-defined maximum length.
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNotFound, this, String.Format(RepositoryExceptionMessage.TooLong_1, fileName), exception);
            }
            catch (NotSupportedException)
            {
                // Thrown if part of the fileName is in an invalid format.
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNotFound, this, String.Format(RepositoryExceptionMessage.InvalidFormat_1, fileName), exception);
            }
            catch (DirectoryNotFoundException)
            {
                if (!NetworkInterface.GetIsNetworkAvailable())
                {
                    throw new RepositoryIOException(IOExceptionCategory.NetworkUnavailable, RepositoryExceptionMessage.NetworkUnavailableWait, this, String.Format(RepositoryExceptionMessage.NetworkUnavailable_1, filePath), exception);
                }
                else
                {
                    // Assume something else besides the network is at fault.
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidHost, this, String.Format(RepositoryExceptionMessage.HostNotFound_1, filePath), exception);
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.UnauthorizedAccess, this, String.Format(RepositoryExceptionMessage.PermissionDenied_1, filePath), exception);
            }
            catch (IOException)
            {
                // If an IOException makes it to here, then something in the underlying mechanism went wrong.
                throw new RepositoryIOException(IOExceptionCategory.General, 60, this, String.Format(RepositoryExceptionMessage.MechanismFailed_1, filePath), exception);
            }
            catch (Exception)
            {
                // If the exception is not recognized above we have an implementation exception.
                throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, RepositoryExceptionMessage.UnrecognizedException, exception);
            }
        }

        private string FullPath => Arguments.Path;

        #region IDirectoryRepository

        /// <inheritdoc/>
        public void CopyFile(string fileName, string newFileName)
        {
            string filePath = GetFilePath(fileName);
            string newFilePath = GetFilePath(newFileName);

            try
            {
                File.Copy(filePath, newFilePath);
                File.SetAccessControl(newFilePath, FilePermissions.CreateFilePermissions());
            }
            catch (Exception ex)
            {
                //TODO Add Exception handling to CopyFile Method
                throw;
            }
        }

        /// <inheritdoc/>
        public Stream CreateFile(string fileName, FileCreationMode fileCreationMode)
        {
            fileName = fileName.Replace('/', '\x2215');

            string filePath = GetFilePath(fileName);

            int tries = 0;

            while (tries < 5)
            {
                try
                {
                    FileMode fileMode = (fileCreationMode == FileCreationMode.ThrowIfFileExists) ? FileMode.CreateNew : (fileCreationMode == FileCreationMode.Overwrite) ? FileMode.Create : FileMode.Append;

                    return new FileStream(filePath, fileMode, System.Security.AccessControl.FileSystemRights.Write, FileShare.Read, 4096, FileOptions.None, FilePermissions.CreateFilePermissions());
                }
                catch (Exception exception)
                {
                    tries++;

                    if (tries >= 5)
                        return HandleException(exception, filePath, fileName);

                    Thread.Sleep(3000);
                    
                }
            }

            return HandleException(new IOException(), filePath, fileName);
        }

        /// <inheritdoc />
        public void DeleteFile(string fileName)
        {
            string filePath = GetFilePath(fileName);

            try
            {
                File.Delete(filePath);
            }
            catch (IOException exception)
            {
                // Thrown if the specified file is in use
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileLocked, this, String.Format(RepositoryExceptionMessage.FileLocked_1, filePath), exception);
            }
            catch (Exception exception)
            {
                HandleException(exception, filePath, fileName);
            }
        }

        /// <inheritdoc/>
        public IEnumerable<DirectoryObjectMetadata> ListFiles(string path = "")
        {
            //If Path is trying to go above Base Directory throw error.
            if (path.Contains("../"))
                throw new ConfigurationFileException("Directory path was above base Path in " + Name);

            string fullpath;
            if (path == "." || path == "/" || path == "")
            {
                fullpath = FullPath;
            }
            else
            {
                if (path.First() == '\\')
                    fullpath = FullPath + path;
                else
                    fullpath = Path.Combine(FullPath, path);
            }

            return System.IO.Directory.EnumerateFiles(fullpath).Select(f => new FileInfo(f)).Select(f => new DirectoryObjectMetadata
            {
                Name = f.Name,
                Path = Path.Combine(fullpath, f.Name),
                ObjectType = DirectoryObjectType.File,
                ModifiedDate = f.LastWriteTime
            }) /*.Union(System.IO.Directory.EnumerateDirectories(fullPath).Select(d => new DirectoryInfo(d)).Select(d => new DirectoryObjectMetadata()
            {
                Name = d.Name,
                Path = Path.Combine(path, d.Name),
                ObjectType = DirectoryObjectType.Directory,
                ModifiedDate = d.LastWriteTime
            }))*/;
        }

        /// <inheritdoc />
        public void MoveFile(string fileName, string newFileName)
        {
            string filePath = GetFilePath(fileName);
            string newFilePath = GetFilePath(newFileName);

            try
            {
                File.Move(filePath, newFilePath);
                File.SetAccessControl(newFilePath, FilePermissions.CreateFilePermissions());
            }
            catch (PathTooLongException exception)
            {
                // Determine which is the longer file name
                string longFileName = (fileName.Length > newFileName.Length) ? fileName : newFileName;
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNameTooLong, this, String.Format(RepositoryExceptionMessage.TooLong_1, longFileName), exception);
            }
            catch (FileNotFoundException exception)
            {
                // Thrown when the original file could not be found.
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNotFound, this, String.Format(RepositoryExceptionMessage.FileNotFound_1, filePath), exception);
            }
            catch (IOException exception)
            {
                // Thrown when the destination file already exists.
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileExists, this, String.Format(RepositoryExceptionMessage.FileAlreadyExists_1, newFilePath), exception);
            }
            catch (UnauthorizedAccessException exception)
            {
                // Thrown if the caller does not have the required permission.
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.UnauthorizedAccess, this, RepositoryExceptionMessage.PermissionDenied_1, exception);
            }
        }

        /// <inheritdoc />
        public Stream OpenFile(string fileName)
        {
            string filePath = GetFilePath(fileName);

            try
            {
                return new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            }
            catch (FileNotFoundException exception)
            {
                // Thrown when the original file could not be found.
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNotFound, this, String.Format(RepositoryExceptionMessage.FileNotFound_1, filePath), exception);
            }
            catch (Exception exception)
            {
                return HandleException(exception, filePath, fileName);
            }
        }

        /// <inheritdoc/>
        public override string ConnectionString => $"Folder | Path = {_arguments.Path}";

        /// <inheritdoc/>
        public void CreateDirectory(string fileName)
        {
            string DirectoryPath = GetFilePath(fileName);

            try
            {
                System.IO.Directory.CreateDirectory(DirectoryPath);
            }
            catch (Exception exception)
            {
                HandleException(exception, DirectoryPath, fileName);
            }
        }

        /// <inheritdoc/>
        public void DeleteDirectory(string fileName)
        {
            string filePath = GetFilePath(fileName);

            try
            {
                System.IO.Directory.Delete(filePath, true);
            }
            catch (IOException exception)
            {
                // Thrown if the specified directory is not empty
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileLocked, this, String.Format(RepositoryExceptionMessage.FileLocked_1, filePath), exception);
            }
            catch (Exception exception)
            {
                HandleException(exception, filePath, fileName);
            }
        }

        #endregion

        #region IDisposable

        public override void Dispose()
        {

        }

        #endregion IDisposable
    }
}