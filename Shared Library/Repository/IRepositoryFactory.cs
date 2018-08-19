using System;

namespace ZondervanLibrary.SharedLibrary.Repository
{
    /// <summary>Provides an interface for creating repositories where the entity type is not know until runtime.</summary>
    /// <typeparam name="TDataContext">The type of data context to create a repository for.</typeparam>
    /// <remarks>
    ///     <para>Unfortunately the <see cref="Leviton.Otis.SharedLibrary.Factory.IFactory{TEntity}"/> interface cannot be used here as the entity type will not be know until runtime.</para>
    /// </remarks>
    public interface IRepositoryFactory<TDataContext>
        where TDataContext : IDataContext
    {
        /// <summary>
        /// Creates an instance of <see cref="IRepository{TEntity}"/>.
        /// </summary>
        /// <typeparam name="TEntity">The type of the entity to create.</typeparam>
        /// <returns>An instance of <see cref="IRepository{TEntity}"/>.</returns>
        IRepository<TEntity> CreateInstance<TEntity>()
            where TEntity : class;


        IRepository CreateInstance(Type entityType);
    }
}
