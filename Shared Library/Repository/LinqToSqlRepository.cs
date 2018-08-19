using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.IO;

namespace ZondervanLibrary.SharedLibrary.Repository
{
    public abstract class LinqToSqlRepositoryBase : IRepositoryBase
    {
        private Boolean _disposed;
        protected readonly IDataContext _dataContext;

        public LinqToSqlRepositoryBase(IDataContext dataContext)
        {
            _dataContext = dataContext ?? throw Argument.NullException(() => dataContext);
            _disposed = false;
        }

        /// <summary>
        /// Finalizes this instance by disposing.
        /// </summary>
        ~LinqToSqlRepositoryBase()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public ChangeConflictCollection ChangeConflicts => _dataContext.ChangeConflicts;

        /// <inheritdoc />
        public Int32 CommandTimeout
        {
            get => _dataContext.CommandTimeout;
            set => _dataContext.CommandTimeout = value;
        }

        /// <inheritdoc />
        public TextWriter Log
        {
            get => _dataContext.Log;
            set => _dataContext.Log = value;
        }

        /// <inheritdoc />
        public void SubmitChanges()
        {
            _dataContext.SubmitChanges();
        }

        /// <inheritdoc />
        public void SubmitChanges(ConflictMode failureMode)
        {
            _dataContext.SubmitChanges(failureMode);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);

            GC.SuppressFinalize(this);
        }

        private void Dispose(Boolean disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                // Free managed resources
                _dataContext.Dispose();
            }

            // Free unmanaged resources

            _disposed = true;
        }
    }

    public class LinqToSqlRepository : LinqToSqlRepositoryBase, IRepository
    {
        //private Boolean _disposed;
        private readonly ITable _dataTable;

        public LinqToSqlRepository(Type dataModelType, IDataContext dataContext)
            : base(dataContext)
        {
            if (dataModelType == null)
                throw Argument.NullException(() => dataModelType);

            try
            {
                _dataTable = _dataContext.GetTable(dataModelType);
            }
            catch (InvalidOperationException exception)
            {
                throw Argument.Exception(() => dataContext, String.Format("{{0}} does not contain a table for type {0}", dataModelType.Name, exception));
            }
        }

        /// <inheritdoc />
        public IQueryable AsQueryable()
        {
            return _dataTable.AsQueryable();
        }

        /// <inheritdoc />
        public void Attach(Object entity)
        {
            _dataTable.Attach(entity);
        }

        /// <inheritdoc />
        public void Attach(Object entity, Boolean asModified)
        {
            _dataTable.Attach(entity, asModified);
        }

        /// <inheritdoc/>
        public void Attach(Object entity, Object original)
        {
            _dataTable.Attach(entity, original);
        }

        /// <inheritdoc/>
        public void AttachAll(IEnumerable entities)
        {
            _dataTable.AttachAll(entities);
        }

        /// <inheritdoc/>
        public void AttachAll(IEnumerable entities, Boolean asModified)
        {
            _dataTable.AttachAll(entities, asModified);
        }

        /// <inheritdoc/>
        public void DeleteAllOnSubmit(IEnumerable entities)
        {
            _dataTable.DeleteAllOnSubmit(entities);
        }

        /// <inheritdoc/>
        public void DeleteOnSubmit(Object entity)
        {
            _dataTable.DeleteOnSubmit(entity);
        }

        /// <inheritdoc/>
        public void InsertAllOnSubmit(IEnumerable entities)
        {
            _dataTable.InsertAllOnSubmit(entities);
        }

        /// <inheritdoc/>
        public void InsertOnSubmit(Object entity)
        {
            _dataTable.InsertOnSubmit(entity);
        }
    }

    /// <summary>
    /// Provides an implementation of <see cref="IRepository{TEntity}"/> using LINQ to SQL to persist changes to a SQL Server database.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to interact with.</typeparam>
    public class LinqToSqlRepository<TEntity> : LinqToSqlRepositoryBase, IRepository<TEntity>
        where TEntity : class
    {
        private readonly Table<TEntity> _dataTable;

        /// <summary>
        /// Creates a new instance of <see cref="LinqToSqlRepository{TEntity}"/> with the given <see cref="IDataContext"/>.
        /// </summary>
        /// <param name="dataContext"></param>
        /// <exception cref="System.ArgumentException"><paramref name="dataContext"/> does not contain a table of type <typeparamref name="TEntity"/>.</exception>
        public LinqToSqlRepository(IDataContext dataContext)
            : base(dataContext)
        {
            try
            {
                _dataTable = _dataContext.GetTable<TEntity>();
            }
            catch (InvalidOperationException exception)
            {
                throw Argument.Exception(() => dataContext, $"{{0}} does not contain a table for type {typeof(TEntity).Name}", exception);
            }
        }

        /// <inheritdoc />
        public IQueryable<TEntity> AsQueryable()
        {
            return _dataTable.AsQueryable();
        }

        /// <inheritdoc />
        public void Attach(TEntity entity)
        {
            _dataTable.Attach(entity);
        }

        /// <inheritdoc />
        public void Attach(TEntity entity, Boolean asModified)
        {
            _dataTable.Attach(entity, asModified);
        }

        /// <inheritdoc />
        public void Attach(TEntity entity, TEntity original)
        {
            _dataTable.Attach(entity, original);
        }

        /// <inheritdoc />
        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            _dataTable.AttachAll(entities);
        }

        /// <inheritdoc />
        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, Boolean asModified)
            where TSubEntity : TEntity
        {
            _dataTable.AttachAll(entities, asModified);
        }

        /// <inheritdoc />
        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            _dataTable.DeleteAllOnSubmit(entities);
        }

        /// <inheritdoc />
        public void DeleteOnSubmit(TEntity entity)
        {
            _dataTable.DeleteOnSubmit(entity);
        }

        /// <inheritdoc />
        public void InsertAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            _dataTable.InsertAllOnSubmit(entities);
        }

        /// <inheritdoc />
        public void InsertOnSubmit(TEntity entity)
        {
            _dataTable.InsertOnSubmit(entity);
        }
    }
}
