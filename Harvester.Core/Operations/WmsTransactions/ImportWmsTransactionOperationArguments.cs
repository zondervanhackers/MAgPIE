using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsTransactions
{
    [XmlType("ImportWmsTransaction")]
    public class ImportWmsTransactionOperationArguments : OperationArgumentsBase
    {
        public string SourceDirectory { get; set; }

        public string DestinationDatabase { get; set; }

        public string HarvesterDatabase { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            ImportWmsTransactionOperationArguments transactionArgs = (ImportWmsTransactionOperationArguments) args;

            return HarvesterDatabase == transactionArgs.HarvesterDatabase
                    && DestinationDatabase == transactionArgs.DestinationDatabase
                    && SourceDirectory == transactionArgs.SourceDirectory;
        }
    }
}
