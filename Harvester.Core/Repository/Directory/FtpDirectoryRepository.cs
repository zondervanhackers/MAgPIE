using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.FtpClient;
using System.Net.Sockets;
using System.IO;

using ZondervanLibrary.Harvester.Core.Exceptions;

namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    public class FtpDirectoryRepository : RepositoryBase, IDirectoryRepository
    {
        private readonly FtpDirectoryRepositoryArguments _arguments;
        private readonly FtpClient _ftpClient;

        public FtpDirectoryRepository(FtpDirectoryRepositoryArguments arguments)
        {
            Contract.Requires(arguments != null);

            Contract.Requires(!String.IsNullOrWhiteSpace(arguments.Host));
            Contract.Requires(!String.IsNullOrWhiteSpace(arguments.Username));
            Contract.Requires(!String.IsNullOrWhiteSpace(arguments.Password));

            _arguments = arguments;
            RepositoryId = new Guid();

            _ftpClient = new FtpClient
            {
                Host = _arguments.Host,
                Credentials = new NetworkCredential(_arguments.Username, _arguments.Password)
            };

            HandleExceptions(() => _ftpClient.Connect(), null);
        }

        #region Helper Functions

        private void HandleExceptions(Action action, String fileName)
        {
            HandleExceptions(() => { action(); return 0; }, fileName);
        }

        private T HandleExceptions<T>(Func<T> action, String fileName)
        {
            try
            {
                return action();
            }
            catch (SocketException exception)
            {
                if (exception.Message.StartsWith("No such host is known"))
                {
                    try
                    {
                        // Basic test for internet access
                        Dns.GetHostAddresses("http://google.com");
                    }
                    catch (SocketException)
                    {
                        throw new RepositoryIOException(IOExceptionCategory.NetworkUnavailable, RepositoryExceptionMessage.NetworkUnavailableWait, this, String.Format(RepositoryExceptionMessage.InternetUnavailable_1, _arguments.Host), exception);
                    }

                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidHost, this, String.Format(RepositoryExceptionMessage.HostNotFound_1, _arguments.Host), exception);
                }
                else
                {
                    throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, RepositoryExceptionMessage.UnrecognizedException, exception);
                }
            }
            catch (FtpCommandException exception)
            {
                switch (exception.CompletionCode)
                {
                    case "530":
                        // Authentication Error
                        throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidCredentials, this, RepositoryExceptionMessage.AuthenticationError, exception);
                    case "550":
                        if (exception.Message.Contains("No such file or directory"))
                        {
                            // File could not be found
                            throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNotFound, this, String.Format(RepositoryExceptionMessage.FileNotFound_1, fileName), exception);
                        }
                        else if (exception.Message.Contains("Permission denied"))
                        {
                            // Permission denied
                            throw new RepositoryConfigurationException(ConfigurationExceptionCategory.UnauthorizedAccess, this, String.Format(RepositoryExceptionMessage.PermissionDenied_1, fileName), exception);
                        }
                        else if (exception.Message.Contains("Failed to open file"))
                        {
                            throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileLocked, this, String.Format(RepositoryExceptionMessage.FileLocked_1, fileName), exception);
                        }
                        break;
                }

                throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, RepositoryExceptionMessage.UnrecognizedError, exception);
            }
            catch (Exception exception)
            {
                throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, RepositoryExceptionMessage.UnrecognizedException, exception);
            }
        }

        #endregion

        #region IDirectoryRepository

        /// <inheritdoc/>
        /// <remarks>There is not good way to copy files in the FTP protcol.  Our solution will essentially "download" the file and resubmit it.</remarks>
        public void CopyFile(string fileName, string newFileName)
        {
            if (_ftpClient.FileExists(newFileName))
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileExists, this, String.Format(RepositoryExceptionMessage.FileAlreadyExists_1, newFileName));
            }

            HandleExceptions(() =>
            {
                using (Stream inputFileStream = _ftpClient.OpenRead(fileName))
                using (Stream outputFileStream = _ftpClient.OpenWrite(newFileName))
                {
                    inputFileStream.CopyTo(outputFileStream);
                }
            }, fileName);
        }

        /// <inheritdoc/>
        public Stream CreateFile(string fileName, FileCreationMode fileMode)
        {
            if (fileMode == FileCreationMode.ThrowIfFileExists && _ftpClient.FileExists(fileName))
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileExists, this, String.Format(RepositoryExceptionMessage.FileAlreadyExists_1, fileName));
            }

            return HandleExceptions(() =>
            {
                if (fileMode == FileCreationMode.Append)
                {
                    return _ftpClient.OpenAppend(fileName);
                }
                else
                {
                    return _ftpClient.OpenWrite(fileName);
                }
            }, fileName);
        }

        /// <inheritdoc/>
        public void DeleteFile(string fileName)
        {
            HandleExceptions(() => _ftpClient.DeleteFile(fileName), fileName);
        }

        /// <inheritdoc/>
        public IEnumerable<DirectoryObjectMetadata> ListFiles(String path)
        {
            //If Path is trying to go above Base Directory throw error.
            if (path.Contains("../"))
                throw new ConfigurationFileException("Directory path was above base Path in " + Name);

            //             (.) = Base path    | (/) = DirectoryPath                      | (/../..) = Base path + appended path
            String fullPath = (path == ".") ? "" : (path == "/") ? _arguments.DirectoryPath : path;

            return _ftpClient.GetListing(fullPath, FtpListOption.Modify).Select(i => new DirectoryObjectMetadata
            {
                Name = i.Name,
                Path = Path.Combine(path, i.Name),
                ObjectType = (i.Type == FtpFileSystemObjectType.Directory) ? DirectoryObjectType.Directory : DirectoryObjectType.File, 
                ModifiedDate = i.Modified
            });
        }

        /// <inheritdoc/>
        public void MoveFile(string fileName, string newFileName)
        {
            if (_ftpClient.FileExists(newFileName))
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileExists, this, String.Format(RepositoryExceptionMessage.FileAlreadyExists_1, newFileName));
            }

            HandleExceptions(() => _ftpClient.Rename(fileName, newFileName), fileName);
        }

        /// <inheritdoc/>
        public Stream OpenFile(string fileName)
        {
            return HandleExceptions(() => _ftpClient.OpenRead(fileName), fileName);
        }

        /// <inheritdoc/>
        public override string ConnectionString => $"FTP | Host = {_arguments.Host}, Port = {_arguments.Port}, UseSSL = {_arguments.UseSsl}, Username = {_arguments.Username}";

        /// <inheritdoc/>
        public void CreateDirectory(string FileName)
        {
            HandleExceptions(() =>
            {
                _ftpClient.CreateDirectory(FileName);
            }, FileName);
        }

        /// <inheritdoc/>
        public void DeleteDirectory(string fileName)
        {
            HandleExceptions(() =>
            {
                _ftpClient.DeleteDirectory(fileName);
            }, fileName);
        }

        #endregion

        #region IDisposable

        /// <inheritdoc/>
        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(Boolean disposing)
        {
            if (disposing)
            {
                // Free Managed Resources
                _ftpClient?.Dispose();
            }

            // Free Unmanaged Resources
        }

        #endregion IDisposable
    }
}
