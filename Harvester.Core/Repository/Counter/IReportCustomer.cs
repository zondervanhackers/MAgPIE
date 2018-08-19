using ZondervanLibrary.Harvester.Core.Repository.Counter;
// ReSharper disable CoVariantArrayConversion

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface IReportCustomer
    {
        IReportItem[] ReportItems { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1
{
    public partial class ReportCustomer : IReportCustomer
    {
        IReportItem[] IReportCustomer.ReportItems { get => ReportItems; set => throw new System.NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.ScholarlyIQSushi
{
    public partial class ReportCustomer : IReportCustomer
    {
        IReportItem[] IReportCustomer.ReportItems { get => ReportItems; set => throw new System.NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.MPSInsight
{
    public partial class ReportCustomer : IReportCustomer
    {
        IReportItem[] IReportCustomer.ReportItems { get => ReportItems; set => throw new System.NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.Gale
{
    public partial class ReportCustomer : IReportCustomer
    {
        IReportItem[] IReportCustomer.ReportItems { get => ReportItems; set => throw new System.NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_4_0
{
    public partial class ReportCustomer : IReportCustomer
    {
        IReportItem[] IReportCustomer.ReportItems { get => ReportItems; set => throw new System.NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_3_0
{
    public partial class ReportCustomer : IReportCustomer
    {
        IReportItem[] IReportCustomer.ReportItems { get => ReportItems; set => throw new System.NotImplementedException(); }
    }
}