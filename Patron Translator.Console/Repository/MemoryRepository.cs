using System;
using System.Collections.Generic;
using System.Data.Linq;

namespace ZondervanLibrary.PatronTranslator.Console.Repository
{
    public class MemoryRepository<TEntity> : ConsumingRepository<TEntity>
        where TEntity : class
    {
        public MemoryRepository(IList<TEntity> dataSource)
        {
            _dataSource = dataSource ?? throw new ArgumentNullException(nameof(dataSource));
        }

        protected override void PopulateDataSource()
        {
            throw new NotImplementedException();
        }

        public override void SubmitChanges()
        {
            SubmitChanges(ConflictMode.FailOnFirstConflict);
        }

        public override void SubmitChanges(ConflictMode failureMode)
        {
            ApplyChanges(failureMode);
        }
    }
}
