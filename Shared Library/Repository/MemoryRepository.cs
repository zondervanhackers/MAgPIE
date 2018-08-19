using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;
using System.IO;

namespace ZondervanLibrary.SharedLibrary.Repository
{
    /// <summary>
    /// Provides base functionality for the strongly and weakly typed MemoryRepository classes.
    /// </summary>
    public abstract class MemoryRepositoryBase : IRepositoryBase
    {
        protected List<Action> _changeQueue;

        public MemoryRepositoryBase()
        {
            _changeQueue = new List<Action>();
        }

        /// <inheritdoc />
        public ChangeConflictCollection ChangeConflicts => throw new NotImplementedException();

        /// <inheritdoc />
        public Int32 CommandTimeout { get; set; }

        /// <inheritdoc />
        public TextWriter Log { get; set; }

        /// <inheritdoc cref="IRepository{TEntity}.SubmitChanges()"/>
        public void SubmitChanges()
        {
            foreach (Action action in _changeQueue)
            {
                action();
            }

            _changeQueue.Clear();
        }

        /// <inheritdoc cref="IRepository{TEntity}.SubmitChanges(ConflictMode)"/>
        public void SubmitChanges(ConflictMode failureMode)
        {
            throw new NotImplementedException();
        }

        // Fufill IDisposable contract.
        public void Dispose()
        { }
    }

    public class MemoryRepository : MemoryRepositoryBase, IRepository
    {
        private readonly ArrayList _sourceTable;

        public MemoryRepository(IEnumerable sourceTable)
        {
            _sourceTable = new ArrayList();

            if (sourceTable != null)
            {
                foreach (Object item in sourceTable)
                {
                    _sourceTable.Add(item);
                }
            }
        }

        /// <inheritdoc/>
        public IQueryable AsQueryable()
        {
            return _sourceTable.AsQueryable();
        }

        /// <inheritdoc />
        public void Attach(Object entity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Attach(Object entity, Boolean asModified)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void Attach(Object entity, Object original)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void AttachAll(IEnumerable entities)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void AttachAll(IEnumerable entities, Boolean asModified)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public void DeleteAllOnSubmit(IEnumerable entities)
        {
            if (entities == null)
                throw Argument.NullException(() => entities);

            foreach (Object entity in entities)
            {
                DeleteOnSubmit(entity);
            }
        }

        /// <inheritdoc/>
        public void DeleteOnSubmit(Object entity)
        {
            if (entity == null)
                throw Argument.NullException(() => entity);

            if (!_sourceTable.Contains(entity))
                throw new InvalidOperationException("Cannot remove an entity that has not been attached.");

            _changeQueue.Add(() =>
            {
                _sourceTable.Remove(entity);
            });
        }

        /// <inheritdoc/>
        public void InsertAllOnSubmit(IEnumerable entities)
        {
            if (entities == null)
                throw Argument.NullException(() => entities);

            foreach (Object entity in entities)
            {
                InsertOnSubmit(entity);
            }
        }

        /// <inheritdoc/>
        public void InsertOnSubmit(Object entity)
        {
            if (entity == null)
                throw Argument.NullException(() => entity);

            if (_sourceTable.Contains(entity))
                throw new InvalidOperationException("Cannot add an entity that already exists.");

            _changeQueue.Add(() =>
            {
                _sourceTable.Add(entity);
            });
        }
    }

    /// <summary>
    /// Provides an in-memory repository used primarily for testing purposes as it will not persist beyond its lifetime.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity to interact with.</typeparam>
    public class MemoryRepository<TEntity> : MemoryRepositoryBase, IRepository<TEntity>
        where TEntity : class
    {
        private readonly List<TEntity> _sourceTable;

        /// <summary>
        /// Creates a new instance of <see cref="MemoryRepository{TEntity}"/>.
        /// </summary>
        /// <param name="sourceTable">An initial collection of elements to initialize the table with.  If <paramref name="sourceTable"/> is null, then the in memory repository will be empty.</param>
        public MemoryRepository(IEnumerable<TEntity> sourceTable)
        {
            _sourceTable = sourceTable?.ToList() ?? new List<TEntity>();
        }

        /// <inheritdoc cref="IRepository{TEntity}.AsQueryable()"/>
        public IQueryable<TEntity> AsQueryable()
        {
            return _sourceTable.AsQueryable();
        }

        /// <inheritdoc cref="IRepository{TEntity}.Attach(TEntity)"/>
        public void Attach(TEntity entity)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity}.Attach(TEntity, Boolean)"/>
        public void Attach(TEntity entity, Boolean asModified)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity}.Attach(TEntity, TEntity)"/>
        public void Attach(TEntity entity, TEntity original)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity}.AttachAll{TSubEntity}(IEnumerable{TSubEntity})"/>
        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity}.AttachAll{TSubEntity}(IEnumerable{TSubEntity}, Boolean)"/>
        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, Boolean asModified)
            where TSubEntity : TEntity
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc cref="IRepository{TEntity}.DeleteAllOnSubmit{TSubEntity}(IEnumerable{TSubEntity})"/>
        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            if (entities == null)
                throw Argument.NullException(() => entities);

            foreach (TSubEntity entity in entities)
            {
                DeleteOnSubmit(entity);
            }
        }

        /// <inheritdoc cref="IRepository{TEntity}.DeleteOnSubmit(TEntity)"/>
        public void DeleteOnSubmit(TEntity entity)
        {
            if (entity == null)
                throw Argument.NullException(() => entity);

            if (!_sourceTable.Contains(entity))
                throw new InvalidOperationException("Cannot remove an entity that has not been attached.");

            _changeQueue.Add(() =>
            {
                _sourceTable.Remove(entity);
            });
        }

        /// <inheritdoc cref="IRepository{TEntity}.InsertAllOnSubmit{TSubEntity}(IEnumerable{TSubEntity})"/>
        public void InsertAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            if (entities == null)
                throw Argument.NullException(() => entities);

            foreach (TSubEntity entity in entities)
            {
                InsertOnSubmit(entity);
            }
        }

        /// <inheritdoc cref="IRepository{TEntity}.InsertOnSubmit(TEntity)"/>
        public void InsertOnSubmit(TEntity entity)
        {
            if (entity == null)
                throw Argument.NullException(() => entity);

            if (_sourceTable.Contains(entity))
                throw new InvalidOperationException("Cannot add an entity that already exists.");

            _changeQueue.Add(() =>
            {
                _sourceTable.Add(entity);
            });
        }
    }
}
