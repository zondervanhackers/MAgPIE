using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsInventory
{
    [XmlType("ImportWmsInventory")]
    public class ImportWmsInventoryOperationArguments : OperationArgumentsBase
    {
        public string SourceDirectory { get; set; }

        public string DestinationDatabase { get; set; }

        public string HarvesterDatabase { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            ImportWmsInventoryOperationArguments inventoryArgs = (ImportWmsInventoryOperationArguments) args;

            return HarvesterDatabase == inventoryArgs.HarvesterDatabase
                    && DestinationDatabase == inventoryArgs.DestinationDatabase
                    && SourceDirectory == inventoryArgs.SourceDirectory;
        }
    }
}