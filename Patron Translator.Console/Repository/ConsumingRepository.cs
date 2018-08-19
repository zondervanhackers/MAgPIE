using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Data.Linq;

namespace ZondervanLibrary.PatronTranslator.Console.Repository
{
    /// <summary>
    /// Base class for a family of repositories that consume a resource and hold the data in a IList in memory and writes back to the resource on submit changes.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class ConsumingRepository<TEntity> : IRepository<TEntity>
        where TEntity : class
    {
        protected IList<TEntity> _dataSource;
        private readonly List<Action<ConflictMode>> _changeQueue;

        protected abstract void PopulateDataSource();

        public ConsumingRepository()
        {
            _changeQueue = new List<Action<ConflictMode>>();

            CommandTimeout = 30;
            Log = null;
        }

        public ChangeConflictCollection ChangeConflicts => throw new NotImplementedException();

        public Int32 CommandTimeout { get; set; }

        public TextWriter Log { get; set; }

        public IQueryable<TEntity> AsQueryable()
        {
            if (_dataSource == null)
                PopulateDataSource();

            return _dataSource.AsQueryable();
        }

        public void Attach(TEntity entity)
        {
            throw new NotImplementedException();
        }

        public void Attach(TEntity entity, Boolean asModified)
        {
            throw new NotImplementedException();
        }

        public void Attach(TEntity entity, TEntity original)
        {
            throw new NotImplementedException();
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            throw new NotImplementedException();
        }

        public void AttachAll<TSubEntity>(IEnumerable<TSubEntity> entities, Boolean asModified)
            where TSubEntity : TEntity
        {
            throw new NotImplementedException();
        }

        public void DeleteAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (TSubEntity entity in entities)
            {
                DeleteOnSubmit(entity);
            }
        }

        public void DeleteOnSubmit(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (!_dataSource.Contains(entity))
                throw new InvalidOperationException("Cannot remove an entity that has not been attached.");

            _changeQueue.Add(delegate(ConflictMode conflictMode)
            {
                DeleteHelper(entity, conflictMode);
            });
        }

        public void InsertAllOnSubmit<TSubEntity>(IEnumerable<TSubEntity> entities)
            where TSubEntity : TEntity
        {
            if (entities == null)
                throw new ArgumentNullException(nameof(entities));

            foreach (TSubEntity entity in entities)
            {
                InsertOnSubmit(entity);
            }
        }

        public void InsertOnSubmit(TEntity entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            if (_dataSource == null)
                PopulateDataSource();

            if (_dataSource.Contains(entity))
                throw new InvalidOperationException("Cannot add an entity that already exists.");

            _changeQueue.Add(delegate(ConflictMode conflictMode)
            {
                InsertHelper(entity, conflictMode);
            });
        }

        public abstract void SubmitChanges();
        public abstract void SubmitChanges(ConflictMode failureMode);

        protected void ApplyChanges(ConflictMode conflictMode)
        {
            foreach (Action<ConflictMode> action in _changeQueue)
            {
                action(conflictMode);
            }

            _changeQueue.Clear();
        }

        private void InsertHelper(TEntity entity, ConflictMode conflictMode)
        {
            _dataSource.Add(entity);
        }

        private void DeleteHelper(TEntity entity, ConflictMode conflictMode)
        {
            _dataSource.Remove(entity);
        }
    }
}
