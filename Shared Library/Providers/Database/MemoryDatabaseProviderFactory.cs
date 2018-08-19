using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.SharedLibrary.Providers.Database
{
    public class MemoryDatabaseProviderFactory<TDataContext> : IDatabaseProviderFactory<TDataContext>
        where TDataContext : class, IDataContext
    {
        public IDatabaseProvider<TDataContext> CreateInstance()
        {
            return new DatabaseProvider<TDataContext>(null, new MemoryRepositoryFactory<TDataContext>());
        }
    }
}
