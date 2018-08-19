using System;
using System.ServiceModel;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public interface ISushiServiceInterfaceClient : ICommunicationObject, IDisposable
    {
        ICounterReportResponse GetReport(IReportRequest reportRequest);

        IReportRequest GenerateReportRequest(DateTime runDate, string reportName,
            SushiCounterRepositoryArguments arguments, string releaseVersion);
    }
}