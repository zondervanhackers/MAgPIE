using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.SharedLibrary.Providers.Database
{
    public interface IDatabaseProviderFactory<TDataContext>
        where TDataContext : IDataContext
    {
        IDatabaseProvider<TDataContext> CreateInstance();
    }
}