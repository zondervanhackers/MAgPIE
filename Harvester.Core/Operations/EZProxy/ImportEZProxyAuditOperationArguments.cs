using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.EZProxy
{
    [XmlType("ImportEZProxyAudit")]
    public class ImportEZProxyAuditOperationArguments : OperationArgumentsBase
    {
        public string SourceDirectory { get; set; }

        public string LogDirectory { get; set; }

        public string DestinationDatabase { get; set; }

        public string HarvesterDatabase { get; set; }

        public string FilePattern { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            ImportEZProxyAuditOperationArguments arguments = (ImportEZProxyAuditOperationArguments) args;

            return HarvesterDatabase == arguments.HarvesterDatabase
                && DestinationDatabase == arguments.DestinationDatabase
                && SourceDirectory == arguments.SourceDirectory
                && LogDirectory == arguments.LogDirectory
                && FilePattern == arguments.FilePattern;
        }
    }
}
