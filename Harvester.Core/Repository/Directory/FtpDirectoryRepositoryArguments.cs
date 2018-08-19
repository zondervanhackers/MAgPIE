using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    [XmlType("FtpDirectory")]
    public class FtpDirectoryRepositoryArguments : RepositoryArgumentsBase
    {
        public string Host { get; set; }

        public int Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool UseSsl { get; set; }

        public string DirectoryPath { get; set; }
    }
}
