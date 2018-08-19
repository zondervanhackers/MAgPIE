using System;

namespace ZondervanLibrary.Harvester.Core.Repository
{
    /// <summary>
    /// Defines an interface for all repository instances.
    /// </summary>
    public interface IRepository : IDisposable
    {
        Guid RepositoryId { get; }

        String Name { get; }

        /// <summary>
        /// When implemented in a derived class this should provide information about the connection.
        /// </summary>
        /// <remarks>
        ///     <para>This will be used in the logs.</para>
        /// </remarks>
        String ConnectionString { get; }
    }
}
