using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Sockets;
using System.IO;

using Renci.SshNet;
using Renci.SshNet.Common;
using ZondervanLibrary.Harvester.Core.Exceptions;

namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    public class SftpDirectoryRepository : RepositoryBase, IDirectoryRepository
    {
        private readonly SftpDirectoryRepositoryArguments _arguments;
        private readonly SftpClient _sftpClient;

        public SftpDirectoryRepository(SftpDirectoryRepositoryArguments arguments)
        {
            Contract.Requires(arguments != null);

            Contract.Requires(!String.IsNullOrWhiteSpace(arguments.Host));
            Contract.Requires(!String.IsNullOrWhiteSpace(arguments.Username));
            Contract.Requires(!String.IsNullOrWhiteSpace(arguments.Password));

            _arguments = arguments;

            Name = _arguments.Name;
            RepositoryId = new Guid();

            // SSH.NET requires some strange password providing mechanism
            PasswordAuthenticationMethod passwordMethod = new PasswordAuthenticationMethod(_arguments.Username, _arguments.Password);

            KeyboardInteractiveAuthenticationMethod keyboardMethod = new KeyboardInteractiveAuthenticationMethod(_arguments.Username);
            keyboardMethod.AuthenticationPrompt += (sender, e) =>
            {
                foreach (AuthenticationPrompt prompt in e.Prompts)
                {
                    if (prompt.Request.IndexOf("Password:", StringComparison.InvariantCultureIgnoreCase) != -1)
                    {
                        prompt.Response = _arguments.Password;
                    }
                }
            };

            ConnectionInfo connectionInfo = new ConnectionInfo(_arguments.Host, _arguments.Port, _arguments.Username, passwordMethod, keyboardMethod) { Timeout = TimeSpan.FromMinutes(20) };

            _sftpClient = new SftpClient(connectionInfo);

            HandleExceptions(() => _sftpClient.Connect(), null);
            
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
            catch (SshAuthenticationException exception)
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidCredentials, this, RepositoryExceptionMessage.AuthenticationError, exception);
            }
            catch (SftpPathNotFoundException exception)
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileNotFound, this, String.Format(RepositoryExceptionMessage.FileNotFound_1, fileName), exception);
            }
            catch (SftpPermissionDeniedException exception)
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.UnauthorizedAccess, this, String.Format(RepositoryExceptionMessage.PermissionDenied_1, fileName), exception);
            }
            catch (SshConnectionException exception)
            {
                throw new RepositoryIOException(IOExceptionCategory.General, 60, this, RepositoryExceptionMessage.ConnectionClosed, exception);
            }
            catch (SshOperationTimeoutException exception)
            {
                throw new RepositoryIOException(IOExceptionCategory.TimedOut, RepositoryExceptionMessage.TimedOutWait, this, RepositoryExceptionMessage.Timeout, exception);
            }
            catch (SshException exception)
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileExists, this, String.Format(RepositoryExceptionMessage.FileAlreadyExists_1, fileName), exception);
            }
            catch (Exception exception)
            {
                throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, RepositoryExceptionMessage.UnrecognizedException, exception);
            }
        }

        #endregion

        #region IDirectoryRepository

        /// <inheritdoc/>
        /// <remarks>SFTP offers something called symbolic links, but they are not always enabled and are not actually a real copy.</remarks>
        public void CopyFile(string fileName, string newFileName)
        {
            HandleExceptions(() =>
            {
                if (_sftpClient.Exists(newFileName))
                {
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileExists, this, String.Format(RepositoryExceptionMessage.FileAlreadyExists_1, newFileName));
                }

                using (Stream inputFileStream = _sftpClient.OpenRead(fileName))
                using (Stream outputFileStream = _sftpClient.Open(newFileName, FileMode.CreateNew, FileAccess.Write))
                {
                    inputFileStream.CopyTo(outputFileStream);
                }

                _sftpClient.RenameFile(fileName, newFileName);
            }, fileName);
        }

        /// <inheritdoc/>
        public Stream CreateFile(String fileName, FileCreationMode fileMode)
        {
            FileMode fm = fileMode == FileCreationMode.ThrowIfFileExists ? FileMode.CreateNew : fileMode == FileCreationMode.Overwrite ? FileMode.CreateNew : FileMode.Append;

            return HandleExceptions(() => _sftpClient.Open(fileName, fm, FileAccess.Write), fileName);
        }

        /// <inheritdoc/>
        public void DeleteFile(String fileName)
        {
            HandleExceptions(() =>
            {
                _sftpClient.DeleteFile(fileName);
            }, fileName);
        }

        /// <inheritdoc/>
        public IEnumerable<DirectoryObjectMetadata> ListFiles(String path)
        {
            //If Path is trying to go above Base Directory throw error.
            if (path.Contains("../"))
                throw new ConfigurationFileException("Directory path was above base Path in " + Name);

            //             (.) = Base path    | (/) = DirectoryPath                      | (/../..) = Base path + appended path
            String fullPath = (path == ".") ? "" : (path == "/") ? _arguments.DirectoryPath : path;

            return _sftpClient.ListDirectory(fullPath).Where(f => !f.Name.StartsWith(".")).Select(f => new DirectoryObjectMetadata
            {
                Name = f.Name,
                Path = Path.Combine(path, f.Name),
                ObjectType = f.IsDirectory ? DirectoryObjectType.Directory : DirectoryObjectType.File,
                ModifiedDate = f.LastWriteTime
            });
        }

        /// <inheritdoc/>
        public void MoveFile(String fileName, String newFileName)
        {
            HandleExceptions(() =>
            {
                if (_sftpClient.Exists(newFileName))
                {
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.FileExists, this, String.Format(RepositoryExceptionMessage.FileAlreadyExists_1, newFileName));
                }

                _sftpClient.RenameFile(fileName, newFileName);
            }, fileName);
        }

        /// <inheritdoc/>
        public Stream OpenFile(String fileName)
        {
            string pathToFile = Path.Combine(_arguments.DirectoryPath, fileName);
            pathToFile = pathToFile.Replace("\\", "/");

            return HandleExceptions(() => _sftpClient.Open(pathToFile, FileMode.Open, FileAccess.Read), fileName);
        }
        
        /// <inheritdoc/>
        public override string ConnectionString => $"SFTP | Host = {_arguments.Host}, Port = {_arguments.Port}, Username = {_arguments.Username}";

        public void CreateDirectory(String fileName)
        {
            HandleExceptions(() => 
            { 
                _sftpClient.CreateDirectory(fileName); 
            }, fileName);
        }

        public void DeleteDirectory(String fileName)
        {
            HandleExceptions(() =>
            {
                _sftpClient.DeleteDirectory(fileName);
            }, fileName);
        }

        #endregion IDirectoryRepository

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
                // Free managed resources
                _sftpClient?.Dispose();
            }

            // Free unmanaged resources
        }

        #endregion IDisposable
    }
}
