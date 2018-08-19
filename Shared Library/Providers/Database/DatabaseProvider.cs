using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.SharedLibrary.Providers.Database
{
    public class DatabaseProvider<TDataContext> : IDatabaseProvider<TDataContext>
        where TDataContext : IDataContext
    {
        public DatabaseProvider(TDataContext dataContext, IRepositoryFactory<TDataContext> repositoryFactory)
        {
            DataContext = dataContext;
            RepositoryFactory = repositoryFactory;
        }

        public TDataContext DataContext { get; }

        public IRepositoryFactory<TDataContext> RepositoryFactory { get; }

        public void Dispose()
        {
            if (DataContext != null)
            {
                DataContext.Dispose();
            }
        }
    }
}
