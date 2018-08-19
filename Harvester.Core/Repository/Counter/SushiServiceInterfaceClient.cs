using System;
using System.Collections.Generic;
using System.Linq;
using java.lang;
using ZondervanLibrary.Harvester.Core.Repository;
using ZondervanLibrary.Harvester.Core.Repository.Counter;
using ZondervanLibrary.SharedLibrary.Collections;

// ReSharper disable CoVariantArrayConversion

namespace ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1
{
    public partial class SushiServiceInterfaceClient : ISushiServiceInterfaceClient
    {
        public ICounterReportResponse GetReport(IReportRequest reportRequest)
        {
            return GetReport(reportRequest as ReportRequest);
        }

        public IReportRequest GenerateReportRequest(DateTime runDate, string reportName, SushiCounterRepositoryArguments arguments, string releaseVersion)
        {
            return new ReportRequest
            {
                ID = "",
                Created = DateTime.UtcNow,
                Requestor = new Requestor
                {
                    Email = "systems_librarian@taylor.edu",
                    ID = arguments.RequestorID,
                    Name = "Taylor University"
                },
                CustomerReference = new CustomerReference
                {
                    ID = arguments.CustomerID,
                    Name = "Taylor University"
                },
                ReportDefinition = new ReportDefinition
                {
                    Name = reportName,
                    Release = releaseVersion,
                    Filters = new ReportDefinitionFilters
                    {
                        UsageDateRange = LiftMonthRange(runDate),
                        //Filter = new Sushi.FilterName[] { new Sushi.FilterName() { Name = "ExcludeZeroUsage", Value = "False" }}
                    }
                },
                PlatformCorrections = arguments.PlatformCorrections,
            };
        }

        private Range LiftMonthRange(DateTime runDate)
        {
            DateTime startDate = new DateTime(runDate.Year, runDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return new Range { Begin = startDate, End = endDate };
        }
    }

    public partial class ReportResponse : IReportResponse
    {
        IException[] IReportResponse.Exception => Exception;
    }

    public partial class Exception : IException { }
    public partial class Requestor : IRequestor { }
    public partial class CustomerReference : ICustomerReference { }
    public partial class ReportDefinition : IReportDefinition { }

    public partial class CounterReportResponse : ICounterReportResponse
    {
        IException[] IReportResponse.Exception => Exception;

        IReport[] ICounterReportResponse.Report => Report;
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_4_0
{
    public partial class SushiServiceInterfaceClient : ISushiServiceInterfaceClient
    {
        public ICounterReportResponse GetReport(IReportRequest reportRequest)
        {
            return CounterRepositoryHelper.FixItemPlatformIssue(HandleNoData(GetReport(reportRequest as ReportRequest)), reportRequest.PlatformCorrections);
        }

        private static CounterReportResponse HandleNoData(CounterReportResponse response)
        {
            if (response.Exception != null && response.Exception.Length > 0)
                return response;

            if (response.Report.Length == 0)
                response.Report = new[] { new Report { Customer = new[] { new ReportCustomer { ReportItems = new ReportItem[0] } } } };

            if (response.Report[0].Customer == null)
                response.Report[0].Customer = new[] { new ReportCustomer { ReportItems = new ReportItem[0] } };

            return response;
        }

        public IReportRequest GenerateReportRequest(DateTime runDate, string reportName, SushiCounterRepositoryArguments arguments, string releaseVersion)
        {
            return new ReportRequest
            {
                ID = "",
                Created = DateTime.UtcNow,
                Requestor = new Requestor
                {
                    Email = "systems_librarian@taylor.edu",
                    ID = arguments.RequestorID,
                    Name = "Taylor University"
                },
                CustomerReference = new CustomerReference
                {
                    ID = arguments.CustomerID,
                    Name = "Taylor University"
                },
                ReportDefinition = new ReportDefinition
                {
                    Name = reportName,
                    Release = releaseVersion,
                    Filters = new ReportDefinitionFilters
                    {
                        UsageDateRange = LiftMonthRange(runDate),
                        //Filter = new Sushi.FilterName[] { new Sushi.FilterName() { Name = "ExcludeZeroUsage", Value = "False" }}
                    }
                },
                PlatformCorrections = arguments.PlatformCorrections,
            };
        }

        private Range LiftMonthRange(DateTime runDate)
        {
            DateTime startDate = new DateTime(runDate.Year, runDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return new Range { Begin = startDate, End = endDate };
        }
    }

    public partial class ReportResponse : IReportResponse
    {
        IException[] IReportResponse.Exception => Exception;
    }

    public partial class Exception : IException { }
    public partial class Requestor : IRequestor { }
    public partial class CustomerReference : ICustomerReference { }
    public partial class ReportDefinition : IReportDefinition
    {
        Counter.Sushi_4_1.ReportDefinitionFilters IReportDefinition.Filters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    public partial class Report : IReport
    {
        IReportCustomer[] IReport.Customer { get => Customer; set => throw new NotImplementedException(); }
    }

    public partial class CounterReportResponse : ICounterReportResponse
    {
        IException[] IReportResponse.Exception => Exception;

        IReport[] ICounterReportResponse.Report => Report;
    }

}

namespace ZondervanLibrary.Harvester.Core.Repository.Journal.Sushi_3_0
{
    public partial class SushiServiceInterfaceClient : ISushiServiceInterfaceClient, SushiServiceInterface
    {
        public ICounterReportResponse GetReport(IReportRequest reportRequest)
        {
            return GetReport((ReportRequest)reportRequest);
        }

        public IReportRequest GenerateReportRequest(DateTime runDate, string reportName, SushiCounterRepositoryArguments arguments, string releaseVersion)
        {
            return new ReportRequest
            {
                ID = "",
                Created = DateTime.UtcNow,
                Requestor = new Requestor
                {
                    Email = "systems_librarian@taylor.edu",
                    ID = arguments.RequestorID,
                    Name = "Taylor University"
                },
                CustomerReference = new CustomerReference
                {
                    ID = arguments.CustomerID,
                    Name = "Taylor University"
                },
                ReportDefinition = new ReportDefinition
                {
                    Name = reportName,
                    Release = releaseVersion,
                    Filters = new ReportDefinitionFilters
                    {
                        UsageDateRange = LiftMonthRange(runDate),
                        //Filter = new Sushi.FilterName[] { new Sushi.FilterName() { Name = "ExcludeZeroUsage", Value = "False" }}
                    }
                },
                PlatformCorrections = arguments.PlatformCorrections,
            };
        }
        private Range LiftMonthRange(DateTime runDate)
        {
            DateTime startDate = new DateTime(runDate.Year, runDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return new Range { Begin = startDate, End = endDate };
        }
    }


    public partial class CounterReportResponse : ICounterReportResponse
    {
        IException[] IReportResponse.Exception => Exception;

        IReport[] ICounterReportResponse.Report => Report;
    }

    public partial class Report : IReport
    {
        IReportCustomer[] IReport.Customer { get => Customer; set => throw new NotImplementedException(); }
    }

    public partial class ReportResponse : IReportResponse
    {
        IException[] IReportResponse.Exception => Exception;
    }

    public partial class Exception : IException { }
    public partial class Requestor : IRequestor { }
    public partial class CustomerReference : ICustomerReference { }
    public partial class ReportDefinition : IReportDefinition
    {
        Counter.Sushi_4_1.ReportDefinitionFilters IReportDefinition.Filters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.MPSInsight
{
    public partial class SushiServiceInterfaceClient : ISushiServiceInterfaceClient
    {
        public ICounterReportResponse GetReport(IReportRequest reportRequest)
        {
            return GetReport(reportRequest as ReportRequest);
        }

        public IReportRequest GenerateReportRequest(DateTime runDate, string reportName, SushiCounterRepositoryArguments arguments, string releaseVersion)
        {
            return new ReportRequest
            {
                ID = "",
                Created = DateTime.UtcNow,
                Requestor = new Requestor
                {
                    Email = "systems_librarian@taylor.edu",
                    ID = arguments.RequestorID,
                    Name = "Taylor University"
                },
                CustomerReference = new CustomerReference
                {
                    ID = arguments.CustomerID,
                    Name = "Taylor University"
                },
                ReportDefinition = new ReportDefinition
                {
                    Name = reportName,
                    Release = releaseVersion,
                    Filters = new ReportDefinitionFilters
                    {
                        UsageDateRange = LiftMonthRange(runDate),
                        //Filter = new Sushi.FilterName[] { new Sushi.FilterName() { Name = "ExcludeZeroUsage", Value = "False" }}
                    }
                },
                PlatformCorrections = arguments.PlatformCorrections,
            };
        }

        private Range LiftMonthRange(DateTime runDate)
        {
            DateTime startDate = new DateTime(runDate.Year, runDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return new Range { Begin = startDate, End = endDate };
        }
    }

    public partial class ReportResponse : IReportResponse
    {
        IException[] IReportResponse.Exception => Exception;
    }

    public partial class Exception : IException { }
    public partial class Requestor : IRequestor { }
    public partial class CustomerReference : ICustomerReference { }
    public partial class ReportDefinition : IReportDefinition
    {
        Repository.Counter.Sushi_4_1.ReportDefinitionFilters IReportDefinition.Filters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
    public partial class Report : IReport
    {
        IReportCustomer[] IReport.Customer { get => Customer; set => throw new NotImplementedException(); }
    }

    public partial class CounterReportResponse : ICounterReportResponse
    {
        IException[] IReportResponse.Exception => Exception;
        
        IReport[] ICounterReportResponse.Report => Report;
    }
}

namespace ZondervanLibrary.Harvester.Core.ScholarlyIQSushi
{
    public partial class SushiServiceClient : ISushiServiceInterfaceClient
    {
        public ICounterReportResponse GetReport(IReportRequest reportRequest)
        {
            return GetReport((ReportRequest)reportRequest);
        }

        public IReportRequest GenerateReportRequest(DateTime runDate, string reportName, SushiCounterRepositoryArguments arguments, string releaseVersion)
        {
            var s = DateTime.UtcNow.ToString("s");

            return new ReportRequest
            {
                ID = "",
                Created = s,
                Requestor = new Requestor
                {
                    Email = "systems_librarian@taylor.edu",
                    ID = arguments.RequestorID,
                    Name = "Taylor University"
                },
                CustomerReference = new CustomerReference
                {
                    ID = arguments.CustomerID,
                    Name = "Taylor University"
                },
                ReportDefinition = new ReportDefinition
                {
                    Name = reportName,
                    Release = releaseVersion,
                    Filters = new ReportDefinitionFilters
                    {
                        UsageDateRange = LiftMonthRange(runDate),
                        //Filter = new Sushi.FilterName[] { new Sushi.FilterName() { Name = "ExcludeZeroUsage", Value = "False" }}
                    }
                },
                PlatformCorrections = arguments.PlatformCorrections,
            };
        }

        private Range LiftMonthRange(DateTime runDate)
        {
            DateTime startDate = new DateTime(runDate.Year, runDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return new Range { Begin = startDate, End = endDate };
        }
    }

    public partial class ReportResponse : IReportResponse
    {
        // ReSharper disable once CoVariantArrayConversion
        IException[] IReportResponse.Exception => Exception;
    }

    public partial class Exception : IException { }
    public partial class Requestor : IRequestor { }
    public partial class CustomerReference : ICustomerReference { }
    public partial class ReportDefinition : IReportDefinition
    {
        Repository.Counter.Sushi_4_1.ReportDefinitionFilters IReportDefinition.Filters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}

namespace ZondervanLibrary.Harvester.Core.Gale
{
    public partial class SushiServiceInterfaceClient : ISushiServiceInterfaceClient
    {
        public ICounterReportResponse GetReport(IReportRequest reportRequest)
        {
            return CounterRepositoryHelper.FixItemPlatformIssue(GetReport(reportRequest as ReportRequest), reportRequest.PlatformCorrections);
        }

        public IReportRequest GenerateReportRequest(DateTime runDate, string reportName, SushiCounterRepositoryArguments arguments, string releaseVersion)
        {
            return new ReportRequest
            {
                ID = "",
                Created = DateTime.UtcNow,
                Requestor = new Requestor
                {
                    Email = "systems_librarian@taylor.edu",
                    ID = arguments.RequestorID,
                    Name = "Taylor University"
                },
                CustomerReference = new CustomerReference
                {
                    ID = arguments.CustomerID,
                    Name = "taylor"
                },
                ReportDefinition = new ReportDefinition
                {
                    Name = reportName,
                    Release = releaseVersion,
                    Filters = new ReportDefinitionFilters
                    {
                        UsageDateRange = LiftMonthRange(runDate),
                        //Filter = new Sushi.FilterName[] { new Sushi.FilterName() { Name = "ExcludeZeroUsage", Value = "False" }}
                    }
                },
                PlatformCorrections = arguments.PlatformCorrections,
            };
        }

        private Range LiftMonthRange(DateTime runDate)
        {
            DateTime startDate = new DateTime(runDate.Year, runDate.Month, 1);
            DateTime endDate = startDate.AddMonths(1).AddDays(-1);

            return new Range { Begin = startDate, End = endDate };
        }
    }

    public partial class ReportResponse : IReportResponse
    {
        IException[] IReportResponse.Exception => Exception;
    }

    public partial class Exception : IException { }
    public partial class Requestor : IRequestor { }
    public partial class CustomerReference : ICustomerReference { }
    public partial class ReportDefinition : IReportDefinition
    {
        Repository.Counter.Sushi_4_1.ReportDefinitionFilters IReportDefinition.Filters { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }

    public partial class CounterReportResponse : ICounterReportResponse
    {
        IException[] IReportResponse.Exception => Exception;

        IReport[] ICounterReportResponse.Report => Report;
    }
}

namespace ZondervanLibrary.Harvester.Core.Repository
{
    public static class CounterRepositoryHelper
    {
        public static ICounterReportResponse FixItemPlatformIssue(ICounterReportResponse response, PlatformCorrection[] platformCorrection)
        {
            if (response?.Report?[0].Customer?[0].ReportItems == null || platformCorrection == null || platformCorrection.Length == 0)
                return response;

            if (platformCorrection.Length % 2 != 0)
                throw new ArrayIndexOutOfBoundsException();

            Dictionary<string, string> platformCorrections = platformCorrection.ToDictionary(x => x.PlatformtoReplace, x => x.PlatformReplacingIt);

            foreach (var reportItem in response.Report[0].Customer[0].ReportItems)
            {
                if (platformCorrections.ContainsKey(reportItem.ItemPlatform))
                    reportItem.ItemPlatform = platformCorrections[reportItem.ItemPlatform];
            }

            return response;
        }
    }
}