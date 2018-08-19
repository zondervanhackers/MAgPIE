using System;
using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    [XmlType("FolderDirectory")]
    public class FolderDirectoryRepositoryArguments : RepositoryArgumentsBase
    {
        public String Path { get; set; }
    }
}
