using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.EZProxy
{
    [XmlType("ImportEZProxyLog")]
    public class ImportEZProxyLogOperationArguments : OperationArgumentsBase
    {
        public string SourceDirectory { get; set; }

        public string DestinationDatabase { get; set; }

        public string HarvesterDatabase { get; set; }

        public string FilePattern { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            ImportEZProxyLogOperationArguments ezProxyArgs = (ImportEZProxyLogOperationArguments) args;

            return HarvesterDatabase == ezProxyArgs.HarvesterDatabase
                    && DestinationDatabase == ezProxyArgs.DestinationDatabase
                    && SourceDirectory == ezProxyArgs.SourceDirectory;
        }
    }
}
