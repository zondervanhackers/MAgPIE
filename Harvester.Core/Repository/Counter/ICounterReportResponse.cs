using System;
using System.Xml.Serialization;
using ZondervanLibrary.Harvester.Core.Repository.Counter;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface ICounterReportResponse : IReportResponse
    {
        /// <remarks/>
        IReport[] Report { get; }
    }
}

namespace ZondervanLibrary.Harvester.Core.ScholarlyIQSushi
{
    public partial class CounterReportResponse : ICounterReportResponse {
        // ReSharper disable once CoVariantArrayConversion
        IReport[] ICounterReportResponse.Report => Report;
    }

}