using System;

namespace ZondervanLibrary.SharedLibrary.Repository
{
    public class MemoryRepositoryFactory<TDataContext> : IRepositoryFactory<TDataContext>
        where TDataContext : IDataContext
    {
        public MemoryRepositoryFactory()
        {

        }

        public IRepository<TEntity> CreateInstance<TEntity>()
            where TEntity : class
        {
            return new MemoryRepository<TEntity>(null);
        }

        public IRepository CreateInstance(Type entityType)
        {
            return new MemoryRepository(null);
        }

    }
}
