using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.SharedLibrary.Providers.Database
{
    public class LinqToSqlDatabaseProviderFactory<TDataContext> : IDatabaseProviderFactory<TDataContext>
        where TDataContext : class, IDataContext
    {
        private readonly TDataContext _dataContext;
        private readonly IRepositoryFactory<TDataContext> _repositoryFactory;

        public LinqToSqlDatabaseProviderFactory(TDataContext dataContext, IRepositoryFactory<TDataContext> repositoryFactory)
        {
            _dataContext = dataContext;
            _repositoryFactory = repositoryFactory;
        }

        public IDatabaseProvider<TDataContext> CreateInstance()
        {
            return new DatabaseProvider<TDataContext>(_dataContext, _repositoryFactory);
        }
    }
}
