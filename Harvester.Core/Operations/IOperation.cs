using System;

namespace ZondervanLibrary.Harvester.Core.Operations
{
    public interface IOperation
    {
        string Name { get; }

        int OperationID { get; set; }

        void Execute(DateTime runDate, Action<String> logMessage, System.Threading.CancellationToken cancellationToken);
    }
}
