using System;
using System.Xml.Serialization;
using ZondervanLibrary.Harvester.Core.Repository.Directory;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public abstract class ISushiCounterRepositoryArguments : RepositoryArgumentsBase
    {
        public String Url { get; set; }
        public String RequestorID { get; set; }
        public String CustomerID { get; set; }
        public ReleaseVersion ReleaseVersion { get; set; }
        public CounterReport[] AvailableReports { get; set; }
        public FolderDirectoryRepositoryArguments JsonRepository { get; set; }
    }

    [XmlType("SushiCounter")]
    public class SushiCounterRepositoryArguments : ISushiCounterRepositoryArguments
    {
        public string Version { get; set; }

        public PlatformCorrection[] PlatformCorrections { get; set; }
    }

    [XmlType("SushiMpsCounter")]
    public class SushiMpsCounterRepositoryArguments : ISushiCounterRepositoryArguments { }

    [XmlType("SushiMuseCounter")]
    public class SushiMuseCounterRepositoryArguments : ISushiCounterRepositoryArguments { }

    [XmlType("SushiGaleCounter")]
    public class SushiGaleCounterRepositoryArguments : ISushiCounterRepositoryArguments { }

    [XmlType("SushiJstorCounter")]
    public class SushiJstorCounterRepositoryArguments : ISushiCounterRepositoryArguments { }
}