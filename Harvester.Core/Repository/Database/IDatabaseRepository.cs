using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.Harvester.Core.Repository.Database
{
    /// <summary>
    /// Provides an interface for repositories that provide database access.
    /// </summary>
    /// <typeparam name="TDataContext">The type of data context that this database repository needs to provide.</typeparam>
    /// <remarks>
    ///     <para>The type of TDataContext effectively enforces the schema as LINQ to SQL will check the database for consistency.</para>
    /// </remarks>
    public interface IDatabaseRepository<TDataContext> : IRepository
        where TDataContext : IDataContext
    {
        TDataContext DataContext { get; }
    }
}