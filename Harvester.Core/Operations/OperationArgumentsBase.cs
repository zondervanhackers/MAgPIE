using System;
using System.Xml.Serialization;
using ZondervanLibrary.Harvester.Core.Operations.Counter;
using ZondervanLibrary.Harvester.Core.Operations.Demographics;
using ZondervanLibrary.Harvester.Core.Operations.EZProxy;
using ZondervanLibrary.Harvester.Core.Operations.Statista;
using ZondervanLibrary.Harvester.Core.Operations.Sync;
using ZondervanLibrary.Harvester.Core.Operations.WmsInventory;
using ZondervanLibrary.Harvester.Core.Operations.WmsTransactions;
using ZondervanLibrary.Harvester.Core.Scheduling;

namespace ZondervanLibrary.Harvester.Core.Operations
{
    [XmlType("Operation")]
    [XmlInclude(typeof(ImportCounterTransactionsOperationArguments))]
    [XmlInclude(typeof(ImportDemographicsOperationArguments))]
    [XmlInclude(typeof(ImportWmsInventoryOperationArguments))]
    [XmlInclude(typeof(ImportWmsTransactionOperationArguments))]
    [XmlInclude(typeof(SyncOperationArguments))]
    [XmlInclude(typeof(ImportStatistaOperationArguments))]
    [XmlInclude(typeof(ImportEZProxyAuditOperationArguments))]
    [XmlInclude(typeof(ImportEZProxyLogOperationArguments))]
    public abstract class OperationArgumentsBase
    {
        public String Name { get; set; }

        public ScheduleArgumentsBase[] Schedules { get; set; }

        public int MaximumRunsPerDay { get; set; }

        public int MaximumConcurrentlyRunning { get; set; }

        public abstract bool Equals(OperationArgumentsBase args);
    }
}
