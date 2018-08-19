using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.Demographics
{
    [XmlType("ImportDemographics")]
    public class ImportDemographicsOperationArguments : OperationArgumentsBase
    {
        public string SourceDirectory { get; set; }

        public string DestinationDatabase { get; set; }

        public string HarvesterDatabase { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            ImportDemographicsOperationArguments demographicArgs = (ImportDemographicsOperationArguments) args;

            return HarvesterDatabase == demographicArgs.HarvesterDatabase
                && DestinationDatabase == demographicArgs.DestinationDatabase
                && SourceDirectory == demographicArgs.SourceDirectory;
        }
    }
}
