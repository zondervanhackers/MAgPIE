using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Operations.Sync
{
    [XmlType("Sync")]
    public class SyncOperationArguments : OperationArgumentsBase
    {
        public string SourceDirectory { get; set; }

        public string DestinationDirectory { get; set; }

        public string FilePattern { get; set; }

        public override bool Equals(OperationArgumentsBase args)
        {
            SyncOperationArguments syncArgs = (SyncOperationArguments) args;

            return DestinationDirectory == syncArgs.DestinationDirectory
                    && SourceDirectory == syncArgs.SourceDirectory;
        }

        //public Boolean IsRecursive { get; set; }

        //public Boolean CopyIfNewer { get; set; }

        //public Boolean DeleteFromDestination { get; set; }
    }
}
