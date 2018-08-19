using System;
using System.Xml.Serialization;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    [XmlType("EbscoHostCounter")]
    public class EbscoHostCounterRepositoryArguments : RepositoryArgumentsBase
    {
        public String Username { get; set; }

        public String Password { get; set; }

        public SushiCounterRepositoryArguments Sushi { get; set; }
    }
}
