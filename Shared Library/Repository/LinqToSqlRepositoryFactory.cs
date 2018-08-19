using System;

namespace ZondervanLibrary.SharedLibrary.Repository
{
    /// <inheritdoc/>
    public class LinqToSqlRepositoryFactory<TDataContext> : IRepositoryFactory<TDataContext>
        where TDataContext : IDataContext
    {
        private readonly TDataContext _dataContext;

        /// <summary>
        /// Creates a new instance of <see cref="LinqToSqlRepositoryFactory{TDataContext}"/>.
        /// </summary>
        /// <param name="dataContext">The <see cref="IDataContext"/> to pass to <see cref="LinqToSqlRepository{TEntity}.LinqToSqlRepository(IDataContext)"/>.</param>
        public LinqToSqlRepositoryFactory(TDataContext dataContext)
        {
            _dataContext = dataContext;
        }

        /// <inheritdoc/>
        public IRepository<TEntity> CreateInstance<TEntity>()
            where TEntity : class
        {
            return new LinqToSqlRepository<TEntity>(_dataContext);
        }

        /// <inheritdoc/>
        public IRepository CreateInstance(Type entityType)
        {
            return new LinqToSqlRepository(entityType, _dataContext);
        }
    }
}
