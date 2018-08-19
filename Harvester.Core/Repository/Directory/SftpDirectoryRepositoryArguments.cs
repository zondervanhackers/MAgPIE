using System;
using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Repository.Directory
{
    [XmlType("SftpDirectory")]
    public class SftpDirectoryRepositoryArguments : RepositoryArgumentsBase
    {
        public string Host { get; set; }

        public Int32 Port { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public string DirectoryPath { get; set; }
    }
}
