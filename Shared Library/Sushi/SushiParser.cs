//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.ServiceModel;
//using ZondervanLibrary.SharedLibrary.SushiWebReference;
//using r3Sushi = ZondervanLibrary.SharedLibrary.ServiceReference1;

//namespace ZondervanLibrary.SharedLibrary.Sushi
//{
//    public class SushiParser
//    {
//        private readonly int[] _days = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
//        private const string UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.99 Safari/537.36";

//        public CounterReportResponse ParseSushi(string url, string requestorId, string customerId, DateTime rundate, string requestType, NetworkCredential credentials = null)
//        {
//            DateTime currentDate = DateTime.UtcNow;
//            if (rundate.Year % 4 == 0 && (rundate.Year % 100 != 0 || rundate.Year % 400 == 0))
//                _days[1]++;
//            DateTime endDate = rundate;
//            endDate = endDate.AddDays(-endDate.Day);
//            endDate = endDate.AddDays(_days[endDate.Month]);
//            DateTime beginDate = endDate;
//            beginDate = beginDate.AddDays(-endDate.Day + 1);
//            ReportRequest report = new ReportRequest {ID = "", Created = currentDate};
//            Requestor requestor = new Requestor
//            {
//                Email = "hahnski07@gmail.com",
//                ID = requestorId,
//                Name = "Taylor University"
//            };
//            report.Requestor = requestor;

//            CustomerReference custref = new CustomerReference {ID = customerId, Name = "Taylor University"};
//            report.CustomerReference = custref;

//            ReportDefinitionFilters filters = new ReportDefinitionFilters();
//            Range daterange = new Range {Begin = beginDate.ToUniversalTime(), End = endDate.ToUniversalTime()};
//            filters.UsageDateRange = daterange;

//            ReportDefinition reportdef = new ReportDefinition {Filters = filters, Name = requestType, Release = "4"};
//            report.ReportDefinition = reportdef;

//            SushiService service = new SushiService {Url = url, UserAgent = UserAgent};
//            if (credentials != null)
//            {
//                service.PreAuthenticate = true;
//                service.Credentials = credentials;
//            }
//            service.Timeout = 120000;
//            CounterReportResponse response = service.GetReport(report);
//            return response;
//        }

//        public r3Sushi.CounterReportResponse ParseR3Sushi(string url, string requestorId, string customerId, DateTime rundate, string requestType, NetworkCredential credential = null)
//        {
//            DateTime currentDate = DateTime.UtcNow;
//            if (rundate.Year % 4 == 0 && (rundate.Year % 100 != 0 || rundate.Year % 400 == 0))
//                _days[1]++;
//            DateTime endDate = rundate;
//            endDate = endDate.AddDays(-endDate.Day);
//            endDate = endDate.AddDays(_days[endDate.Month]);
//            DateTime beginDate = endDate;
//            beginDate = beginDate.AddDays(-endDate.Day + 1);

//            r3Sushi.ReportRequest report = new r3Sushi.ReportRequest {ID = "", Created = currentDate};

//            r3Sushi.Requestor requestor = new r3Sushi.Requestor
//            {
//                Email = "hahnski07@gmail.com",
//                ID = requestorId,
//                Name = "Taylor University"
//            };
//            report.Requestor = requestor;

//            r3Sushi.CustomerReference custref = new r3Sushi.CustomerReference
//            {
//                ID = customerId,
//                Name = "Taylor University"
//            };
//            report.CustomerReference = custref;

//            r3Sushi.ReportDefinitionFilters filters = new r3Sushi.ReportDefinitionFilters();
//            r3Sushi.Range daterange = new r3Sushi.Range
//            {
//                Begin = beginDate.ToUniversalTime(),
//                End = endDate.ToUniversalTime()
//            };
//            filters.UsageDateRange = daterange;

//            r3Sushi.ReportDefinition reportdef = new r3Sushi.ReportDefinition
//            {
//                Filters = filters,
//                Name = requestType,
//                Release = "3"
//            };
//            report.ReportDefinition = reportdef;
//            r3Sushi.SushiServiceInterfaceClient service;
//            if (credential != null)
//            {
//                var binding = new BasicHttpBinding(BasicHttpSecurityMode.TransportCredentialOnly);
//                binding.Security.Transport.ClientCredentialType = HttpClientCredentialType.Basic;
//                service = new r3Sushi.SushiServiceInterfaceClient(binding, new EndpointAddress(url));
//                service.ClientCredentials.UserName.UserName = credential.UserName;
//                service.ClientCredentials.UserName.Password = credential.Password;
//            }
//            else
//            {
//                service = new r3Sushi.SushiServiceInterfaceClient();
//            }
//            r3Sushi.CounterReportResponse response = service.GetReport(report);
//            return response;
//        }

//        public IEnumerable<SushiRecord> ParseSushiJournals(string url, string requestorId, string customerId, DateTime rundate, NetworkCredential credential)
//        {
//            const string requestType = "JR1";
//            Console.WriteLine("Retrieving Sushi Journal");
//            CounterReportResponse response;
//            bool notCorrectResponse = true;
//            bool counter4Tried = false;
//            bool counter3Tried = false;
//            do
//            {
//                if (!counter4Tried)
//                {
//                    response = ParseSushi(url, requestorId, customerId, rundate, requestType, credential);
//                    counter4Tried = true;
//                }
//                else
//                {
//                    response = ParseR3Sushi(url, requestorId, customerId, rundate, requestType, credential).ToR4Response();
//                    counter3Tried = true;
//                }
//                if (response.Exception != null)
//                {
//                    Console.WriteLine(
//                        !counter3Tried
//                            ? "-------------------------------------\n{0} Error(s) with Release 4 Response\n-------------------------------------"
//                            : "-------------------------------------\n{0} Error(s) with Release 3 Response\n-------------------------------------",
//                        response.Exception.Length);
//                    for (int i = 0; i < response.Exception.Length; i++)
//                    {
//                        Console.WriteLine("Error {0}", i);
//                        Console.WriteLine("Message: " + response.Exception[0].Message);
//                        Console.WriteLine("Error Num: " + response.Exception[0].Number);
//                    }
//                    switch (response.Exception[0].Number)
//                    {
//                        case 1020:
//                            throw new ArgumentException("Client has made too many requests");
//                        case 1010:
//                            throw new ArgumentException("Server is too busy to handle request. Try Later");
//                        case 3020:
//                            throw new ArgumentException("Date Range is Invalid");
//                        case 3030:
//                            throw new ArgumentException("No Data for this date range");
//                    }
//                    if (counter3Tried)
//                    {
//                        Console.WriteLine("Release 3 Failed\n-------------------------------------");
//                        throw new ArgumentException("Tried both Release 4 & 3 but both failed");
//                    }
//                    Console.WriteLine("Release 4 Failed");
//                }
//                else
//                {
//                    Console.WriteLine("(" + requestType + ")Report has no exceptions");
//                    notCorrectResponse = false;
//                }
//            }
//            while (notCorrectResponse);
//            Console.WriteLine("Done Getting Sushi Journal");
//            foreach (ReportItem report in response.Report[0].Customer[0].ReportItems)
//            {
//                SushiRecord record = new SushiRecord();

//                var reportitem = report;
//                record.OnlineISSN = null;
//                record.PrintISSN = null;
//                foreach (Identifier reportIdentifier in reportitem.ItemIdentifier)
//                {
//                    switch (reportIdentifier.Type)
//                    {
//                        case IdentifierType.Print_ISSN:
//                            record.PrintISSN = reportIdentifier.Value;
//                            break;
//                        case IdentifierType.Online_ISSN:
//                            record.OnlineISSN = reportIdentifier.Value;
//                            break;
//                    }
//                }
//                if (record.PrintISSN != null)
//                    record.PrintISSN = record.PrintISSN.Replace("-", "");
//                if (record.OnlineISSN != null)
//                    record.OnlineISSN = record.OnlineISSN.Replace("-", "");
//                record.Name = reportitem.ItemName;
//                record.Vendor = report.ItemPlatform;
//                foreach (PerformanceCounter performanceData in reportitem.ItemPerformance[0].Instance)
//                    if (performanceData.MetricType == MetricType.ft_total)
//                        record.FullText = Convert.ToInt16(performanceData.Count);
//                record.Database = reportitem.ItemPlatform;
//                yield return record;
//            }
//        }

//        public IEnumerable<SushiVendorRecord> ParseSushiDatabases(string url, string requestorId, string customerId, DateTime rundate, NetworkCredential credential)
//        {
//            string requestType = "DB1";
//            Console.WriteLine("Retrieving Sushi Database");
//            CounterReportResponse response;
//            bool notCorrectResponse = true;
//            bool counter4Tried = false;
//            bool counter3Tried = false;
//            do
//            {
//                if (!counter4Tried)
//                {
//                    response = ParseSushi(url, requestorId, customerId, rundate, requestType, credential);
//                    if (response.Exception != null && response.Exception[0].Number == 3000)
//                    {
//                        Console.WriteLine("(" + requestType + ") Is not Supported By This Vendor");
//                        requestType = "PR1";
//                        Console.WriteLine("Attempting Report Type: (" + requestType + ")");
//                    }
//                    response = ParseSushi(url, requestorId, customerId, rundate, requestType, credential);
//                    counter4Tried = true;
//                }
//                else
//                {
//                    requestType = "DB1";
//                    response = ParseR3Sushi(url, requestorId, customerId, rundate, requestType, credential).ToR4Response();
//                    counter3Tried = true;
//                }
//                if (response.Exception != null)
//                {
//                    Console.WriteLine(
//                        !counter3Tried
//                            ? "-------------------------------------\n{0} Error(s) with Release 4 Response\n-------------------------------------"
//                            : "-------------------------------------\n{0} Error(s) with Release 3 Response\n-------------------------------------",
//                        response.Exception.Length);
//                    for (int i = 0; i < response.Exception.Length; i++)
//                    {
//                        Console.WriteLine("Error {0}", i);
//                        Console.WriteLine("Message: " + response.Exception[0].Message);
//                        Console.WriteLine("Error Number: " + response.Exception[0].Number);
//                    }
//                    if (response.Exception[0].Number == 1020)
//                        throw new ArgumentException("Client has made too many requests");
//                    if (response.Exception[0].Number == 1010)
//                        throw new ArgumentException("Server is too busy to handle request. Try Later");
//                    if (response.Exception[0].Number == 3020)
//                        throw new ArgumentException("Date Range is Invalid");
//                    //throw new ArgumentException("(" + "JR1" + ")# of Errors: " + response.Exception.Length);
//                    if (counter3Tried)
//                    {
//                        Console.WriteLine("Release 3 Failed\n-------------------------------------");
//                        throw new ArgumentException("Tried both Release 4 & 3 but both failed");
//                    }
//                    Console.WriteLine("Release 4 Failed");
//                }
//                else
//                {
//                    Console.WriteLine("(" + requestType + ")Report has no exceptions");
//                    notCorrectResponse = false;
//                }
//            }
//            while (notCorrectResponse);

//            Console.WriteLine("Successfully Retrieved Sushi Database");
//            foreach (var reportitem in response.Report[0].Customer[0].ReportItems)
//            {
//                SushiVendorRecord record = new SushiVendorRecord
//                {
//                    Vendor = reportitem.ItemPlatform,
//                    Database = reportitem.ItemName
//                };
//                foreach (Metric reportMetric in reportitem.ItemPerformance)
//                {
//                    foreach (PerformanceCounter performanceData in reportMetric.Instance)
//                    {
//                        switch (performanceData.MetricType)
//                        {
//                            case MetricType.result_click:
//                                record.Result_Clicks = Convert.ToInt32(performanceData.Count);
//                                break;
//                            case MetricType.search_reg:
//                                record.Searches = Convert.ToInt32(performanceData.Count);
//                                break;
//                            case MetricType.record_view:
//                                record.Record_Views = Convert.ToInt32(performanceData.Count);
//                                break;
//                        }
//                    }
//                }
//                record.Timestamp = rundate;
//                yield return record;
//            }
//        }
//    }
//}