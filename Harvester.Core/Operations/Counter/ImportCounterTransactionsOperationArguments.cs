using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.Counter
{
    [XmlType("ImportCounterTransactions")]
    public class ImportCounterTransactionsOperationArguments : OperationArgumentsBase
    {
        public string SourceCounter { get; set; }

        public string DestinationDatabase { get; set; }

        public string HarvesterDatabase { get; set; }

        public string LocalJsonStorage { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            ImportCounterTransactionsOperationArguments counterArgs = (ImportCounterTransactionsOperationArguments) args;

            return DestinationDatabase == counterArgs.DestinationDatabase
                && HarvesterDatabase == counterArgs.HarvesterDatabase
                && SourceCounter == counterArgs.SourceCounter
                && LocalJsonStorage == counterArgs.LocalJsonStorage;
        }
    }
}
