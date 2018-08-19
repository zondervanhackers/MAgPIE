using System;
using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.SharedLibrary.Providers.Database
{
    public interface IDatabaseProvider<TDataContext> : IDisposable
        where TDataContext : IDataContext
    {
        TDataContext DataContext { get; }

        IRepositoryFactory<TDataContext> RepositoryFactory { get; }
    }
}
