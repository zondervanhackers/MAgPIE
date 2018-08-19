using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.Statista
{
    [XmlType("ImportStatista")]
    public class ImportStatistaOperationArguments : OperationArgumentsBase
    {
        public string SourceDirectory { get; set; }

        public string DestinationDatabase { get; set; }

        public string HarvesterDatabase { get; set; }

        public string FilePattern { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            ImportStatistaOperationArguments statistaArgs = (ImportStatistaOperationArguments) args;

            return HarvesterDatabase == statistaArgs.HarvesterDatabase
                    && DestinationDatabase == statistaArgs.DestinationDatabase
                    && SourceDirectory == statistaArgs.SourceDirectory;
        }
    }
}
