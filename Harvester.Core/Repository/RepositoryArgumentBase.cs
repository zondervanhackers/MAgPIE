using System.Xml.Serialization;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;

namespace ZondervanLibrary.Harvester.Core.Repository
{
    [XmlType("Repository")]
    [XmlInclude(typeof(SqlServerDatabaseRepositoryArguments))]
    [XmlInclude(typeof(FolderDirectoryRepositoryArguments))]
    [XmlInclude(typeof(FtpDirectoryRepositoryArguments))]
    [XmlInclude(typeof(SftpDirectoryRepositoryArguments))]
    [XmlInclude(typeof(EbscoHostCounterRepositoryArguments))]
    [XmlInclude(typeof(SushiCounterRepositoryArguments))]
    [XmlInclude(typeof(SushiMuseCounterRepositoryArguments))]
    [XmlInclude(typeof(SushiMpsCounterRepositoryArguments))]
    [XmlInclude(typeof(SushiGaleCounterRepositoryArguments))]
    [XmlInclude(typeof(SushiJstorCounterRepositoryArguments))]
    public abstract class RepositoryArgumentsBase
    {
        /// <summary>
        /// Gets or sets the unique identifier for this repository.
        /// </summary>
        public string Name { get; set; }
    }
}
