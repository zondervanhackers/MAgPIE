using System;
using System.Collections.Generic;
using System.Linq;
using ZondervanLibrary.Harvester.Core.Operations;
using ZondervanLibrary.Harvester.Core.Operations.Counter;
using ZondervanLibrary.Harvester.Core.Operations.Demographics;
using ZondervanLibrary.Harvester.Core.Operations.EZProxy;
using ZondervanLibrary.Harvester.Core.Operations.Statista;
using ZondervanLibrary.Harvester.Core.Operations.Sync;
using ZondervanLibrary.Harvester.Core.Operations.WmsInventory;
using ZondervanLibrary.Harvester.Core.Operations.WmsTransactions;
using ZondervanLibrary.Harvester.Core.Repository;

namespace ZondervanLibrary.Harvester.Service
{
    public static class OperationContextFactory
    {
        private static Dictionary<string, RepositoryArgumentsBase> repositories;

        public static void SetRepositories(RepositoryArgumentsBase[] repositoryArgumentsBases)
        {
            repositories = repositoryArgumentsBases.ToDictionary(x => x.Name);
        }

        public static void SetRepositories(Dictionary<string, RepositoryArgumentsBase> repositoryDictionary)
        {
            repositories = repositoryDictionary;
        }

        public static OperationContext CreateOperationContext(OperationArgumentsBase arguments, IEnumerator<DateTime> enumerator)
        {
            if (repositories == null)
                throw new FieldAccessException("Repositories member was never set");

            switch (arguments)
            {
                case ImportWmsInventoryOperationArguments wmsInventoryOperationArgs:
                    ImportWmsInventoryOperation wmsInventoryOperation = new ImportWmsInventoryOperation(
                            repositories[wmsInventoryOperationArgs.HarvesterDatabase],
                            repositories[wmsInventoryOperationArgs.DestinationDatabase],
                            repositories[wmsInventoryOperationArgs.SourceDirectory])
                        { Name = wmsInventoryOperationArgs.Name };

                    return new OperationContext(wmsInventoryOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };

                case ImportWmsTransactionOperationArguments wmsTransactionOperationArgs:
                    ImportWmsTransactionOperation wmsTransactionOperation = new ImportWmsTransactionOperation(
                            repositories[wmsTransactionOperationArgs.HarvesterDatabase],
                            repositories[wmsTransactionOperationArgs.DestinationDatabase],
                            repositories[wmsTransactionOperationArgs.SourceDirectory])
                        { Name = wmsTransactionOperationArgs.Name };

                    return new OperationContext(wmsTransactionOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };

                case ImportCounterTransactionsOperationArguments counterTransactionOperationArgs:
                    ImportCounterTransactionsOperation counterOperation = new ImportCounterTransactionsOperation(
                            repositories[counterTransactionOperationArgs.DestinationDatabase],
                            repositories[counterTransactionOperationArgs.HarvesterDatabase],
                            repositories[counterTransactionOperationArgs.SourceCounter],
                            repositories.ContainsKey(counterTransactionOperationArgs.LocalJsonStorage) ? repositories[counterTransactionOperationArgs.LocalJsonStorage]: null)
                        { Name = counterTransactionOperationArgs.Name };

                    return new OperationContext(counterOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };

                case ImportDemographicsOperationArguments demographicOperationArgs:
                    ImportDemographicsOperation demographicOperation = new ImportDemographicsOperation(
                            repositories[demographicOperationArgs.HarvesterDatabase],
                            repositories[demographicOperationArgs.DestinationDatabase],
                            repositories[demographicOperationArgs.SourceDirectory])
                        { Name = demographicOperationArgs.Name };

                    return new OperationContext(demographicOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };

                case SyncOperationArguments syncOperationArgs:
                    SyncOperation syncOperation = new SyncOperation(
                            syncOperationArgs,
                            repositories[syncOperationArgs.DestinationDirectory],
                            repositories[syncOperationArgs.SourceDirectory])
                        { Name = syncOperationArgs.Name };

                    return new OperationContext(syncOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };

                case ImportStatistaOperationArguments statistaOperationArgs:
                    ImportStatistaOperation statistaOperation = new ImportStatistaOperation(
                            statistaOperationArgs,
                            repositories[statistaOperationArgs.HarvesterDatabase],
                            repositories[statistaOperationArgs.DestinationDatabase],
                            repositories[statistaOperationArgs.SourceDirectory])
                        { Name = statistaOperationArgs.Name };

                    return new OperationContext(statistaOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };

                case ImportEZProxyAuditOperationArguments auditOperationArgs:
                    ImportEZProxyAuditOperation auditOperation = new ImportEZProxyAuditOperation(
                            auditOperationArgs,
                            repositories[auditOperationArgs.HarvesterDatabase],
                            repositories[auditOperationArgs.DestinationDatabase],
                            repositories[auditOperationArgs.SourceDirectory],
                            repositories[auditOperationArgs.LogDirectory])
                        { Name = auditOperationArgs.Name };

                    return new OperationContext(auditOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };

                case ImportEZProxyLogOperationArguments logOperationArgs:
                    ImportEZProxyLogOperation logOperation = new ImportEZProxyLogOperation(
                            logOperationArgs,
                            repositories[logOperationArgs.HarvesterDatabase],
                            repositories[logOperationArgs.DestinationDatabase],
                            repositories[logOperationArgs.SourceDirectory])
                        { Name = logOperationArgs.Name };

                    return new OperationContext(logOperation, enumerator) { MaximumRunsPerDay = arguments.MaximumRunsPerDay, MaximumConcurrentlyRunning = arguments.MaximumConcurrentlyRunning };
            }

            throw new NotImplementedException($"{arguments.Name} is not a recognized operation");
        }
    }
}
