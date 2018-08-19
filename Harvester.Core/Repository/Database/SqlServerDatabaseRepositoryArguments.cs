using System;
using System.Xml.Serialization; 

namespace ZondervanLibrary.Harvester.Core.Repository.Database
{
    public enum SqlServerAuthenticationMethod
    {
        Windows,
        UsernamePassword
    }

    [XmlType("SqlServerDatabase")]
    public class SqlServerDatabaseRepositoryArguments : RepositoryArgumentsBase
    {
        public String Server { get; set; }

        public String Database { get; set; }

        public SqlServerAuthenticationMethod Authentication { get; set; }

        public String Username { get; set; }

        public String Password { get; set; }
    }
}
 