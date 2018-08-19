using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

using ZondervanLibrary.Harvester.Core.Repository.Database;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.Harvester.Core.Repository.Counter;

namespace ZondervanLibrary.Harvester.Core.Repository
{
    [XmlType("Repository")]
    [XmlInclude(typeof(SqlServerDatabaseRepositoryArguments))]
    [XmlInclude(typeof(FolderDirectoryRepositoryArguments))]
    [XmlInclude(typeof(FtpDirectoryRepositoryArguments))]
    [XmlInclude(typeof(SftpDirectoryRepositoryArguments))]
    [XmlInclude(typeof(SushiCounterRepositoryArguments))]
    [XmlInclude(typeof(EbscoHostCounterRepositoryArguments))]
    public abstract class RepositoryArgumentsBase
    {
        /// <summary>
        /// The unique identifier for this repository.
        /// </summary>
        public String Name { get; set; }
    }
}