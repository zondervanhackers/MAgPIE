using System.Xml.Serialization;
using ZondervanLibrary.Harvester.Core.Repository.Counter;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface IReportRequest
    {
        /// <remarks/>
        IReportDefinition ReportDefinition { get; set; }

        [XmlIgnore]
        PlatformCorrection[] PlatformCorrections { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1
{
    public partial class ReportRequest : IReportRequest
    {
        IReportDefinition IReportRequest.ReportDefinition
        {
            get => ReportDefinition;
            set => ReportDefinition = value as ReportDefinition;
        }

        [XmlIgnore]
        public PlatformCorrection[] PlatformCorrections { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_4_0
{
    public partial class ReportRequest : IReportRequest
    {
        IReportDefinition IReportRequest.ReportDefinition
        {
            get => ReportDefinition;
            set => ReportDefinition = value as ReportDefinition;
        }

        [XmlIgnore]
        public PlatformCorrection[] PlatformCorrections { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_3_0
{
    public partial class ReportRequest : IReportRequest
    {
        IReportDefinition IReportRequest.ReportDefinition
        {
            get => ReportDefinition;
            set => ReportDefinition = value as ReportDefinition;
        }

        [XmlIgnore]
        public PlatformCorrection[] PlatformCorrections { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.ScholarlyIQSushi
{
    public partial class ReportRequest : IReportRequest
    {
        IReportDefinition IReportRequest.ReportDefinition
        {
            get => ReportDefinition;
            set => ReportDefinition = value as ReportDefinition;
        }

        [XmlIgnore]
        public PlatformCorrection[] PlatformCorrections { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.MPSInsight
{
    public partial class ReportRequest : IReportRequest
    {
        IReportDefinition IReportRequest.ReportDefinition
        {
            get => ReportDefinition;
            set => ReportDefinition = value as ReportDefinition;
        }

        [XmlIgnore]
        public PlatformCorrection[] PlatformCorrections { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.Gale
{
    public partial class ReportRequest : IReportRequest
    {
        IReportDefinition IReportRequest.ReportDefinition
        {
            get => ReportDefinition;
            set => ReportDefinition = value as ReportDefinition;
        }

        [XmlIgnore]
        public PlatformCorrection[] PlatformCorrections { get; set; }
    }
}