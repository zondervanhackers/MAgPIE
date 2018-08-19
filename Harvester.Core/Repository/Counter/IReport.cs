using System;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface IReport
    {
        /// <remarks/>
        IReportCustomer[] Customer { get; set; }
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1
{
    public partial class Report : IReport
    {
        IReportCustomer[] IReport.Customer { get => Customer; set => throw new NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.ScholarlyIQSushi
{
    public partial class Report : IReport
    {
        IReportCustomer[] IReport.Customer { get => Customer; set => throw new NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.Gale
{
    public partial class Report : IReport
    {
        IReportCustomer[] IReport.Customer { get => Customer; set => throw new NotImplementedException(); }
    }
}