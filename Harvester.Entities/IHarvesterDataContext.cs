using System.Data.Linq;
using ZondervanLibrary.SharedLibrary.Repository;

namespace ZondervanLibrary.Harvester.Entities
{
    public interface IHarvesterDataContext : IDataContext
    {
        #region Persistance

        Table<DirectoryRecord> DirectoryRecords { get; }

        Table<OperationRecord> OperationRecords { get; }

        Table<Operation> Operations { get; }

        Table<Repository> Repositories { get; }

        Table<CounterOperationRecord> CounterOperationRecords { get; }

        #endregion
    }
}
