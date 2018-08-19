using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics.Contracts;

using ZondervanLibrary.Harvester.Core.Exceptions;

namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    /// <summary>
    /// Provides an interface for repositories that provide file access into a directory.
    /// </summary>
    [ContractClass(typeof(DirectoryRepositoryContracts))]
    public interface IDirectoryRepository : IRepository
    {
        /// <summary>
        /// Copies a file from the specified location to a new location within the repository.
        /// </summary>
        /// <param name="fileName">The path to the file which will be copied.</param>
        /// <param name="newFileName">The path to copy the file to.</param>
        void CopyFile(String fileName, String newFileName);

        /// <summary>
        /// Creates a new file in the repository returning a stream to write to it.
        /// </summary>
        /// <param name="fileName">The name of the file to create.</param>
        /// <param name="fileMode">Describes how to respond if the file already exists.</param>
        /// <returns>A stream that allows writing to the file.</returns>
        Stream CreateFile(String fileName, FileCreationMode fileMode);

        /// <summary>
        /// Deletes the specified folder in the repository.
        /// </summary>
        /// <param name="fileName">The path to the file to delete within the repository.</param>
        /// <exception cref="IOExceptionCategory.NetworkUnavailable">The resource requires network access and the network is current unavailable.</exception>
        /// <exception cref="ImplementationExceptionCategory.UnrecognizedException">An unrecognized exception occured.\n-or-\nThe underlying protocol returned an unrecognized error.</exception>
        void DeleteFile(String fileName);

        /// <summary>
        /// Returns a list of all the objects in the directory.
        /// </summary>
        /// <param name="path">The path (relative to the directory) to the directory to list the files in.</param>
        /// <returns>A collection of all the objects in the specified directory.</returns>
        /// <remarks>
        ///     <para> //(.) = Base path | (/) = DirectoryPath | (/../..) = Base path + appended path</para>
        /// </remarks>
        IEnumerable<DirectoryObjectMetadata> ListFiles(String path = "");

        /// <summary>
        /// Moves the specified to a new location.
        /// </summary>
        /// <param name="fileName">The path to the file to be moved.</param>
        /// <param name="newFileName">The path to move the file to.</param>
        void MoveFile(String fileName, String newFileName);

        /// <summary>
        /// Opens the specified file in the repository.
        /// </summary>
        /// <param name="fileName">The filename to open in the repository.</param>
        /// <returns>A stream that allows reading from the file.</returns>
        Stream OpenFile(String fileName);

        void CreateDirectory(String fileName);

        void DeleteDirectory(String fileName);
    }

    public class DirectoryObjectMetadata
    {
        /// <summary>
        /// The name of the directory object (includes extension if it is a file).
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The full path to this directory object relative to the DirectoryRepository.
        /// </summary>
        public String Path { get; set; }

        /// <summary>
        /// The date at which the object was last modified.
        /// </summary>
        public DateTime ModifiedDate { get; set; }

        /// <summary>
        /// The type of the object found.
        /// </summary>
        /// <remarks>
        ///     <para>Allowing for the discovery of sub directories should allow for a recursive descent.</para>
        /// </remarks>
        public DirectoryObjectType ObjectType { get; set; }
    }

    public enum DirectoryObjectType
    {
        File,
        Directory
    }

    [ContractClassFor(typeof(IDirectoryRepository))]
    internal sealed class DirectoryRepositoryContracts : IDirectoryRepository
    {
        Stream IDirectoryRepository.CreateFile(string fileName, FileCreationMode fileMode)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));

            Contract.Ensures(Contract.Result<Stream>().CanWrite, "Return stream must be writable.");

            return default(Stream);
        }

        void IDirectoryRepository.CopyFile(String fileName, String newFileName)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));
            Contract.Requires(!String.IsNullOrWhiteSpace(newFileName));
        }

        void IDirectoryRepository.DeleteFile(string fileName)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));
        }

        IEnumerable<DirectoryObjectMetadata> IDirectoryRepository.ListFiles(String path)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(path));

            return default(IEnumerable<DirectoryObjectMetadata>);
        }

        void IDirectoryRepository.MoveFile(string fileName, string newFileName)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));
            Contract.Requires(!String.IsNullOrWhiteSpace(newFileName));
        }

        Stream IDirectoryRepository.OpenFile(string fileName)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));

            Contract.Ensures(Contract.Result<Stream>().CanRead, "Return stream must be readable.");

            return default(Stream);
        }

        void IDirectoryRepository.CreateDirectory(string fileName)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));
        }

        void IDirectoryRepository.DeleteDirectory(string fileName)
        {
            Contract.Requires(!String.IsNullOrWhiteSpace(fileName));
        }

        void IDisposable.Dispose()
        {
            
        }

        public Guid RepositoryId => throw new NotImplementedException();

        public string Name => throw new NotImplementedException();

        public string ConnectionString => throw new NotImplementedException();
    }
}
