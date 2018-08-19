using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Text.RegularExpressions;
using ZondervanLibrary.Harvester.Core.Exceptions;
using ZondervanLibrary.Harvester.Core.Repository.Directory;
using ZondervanLibrary.SharedLibrary.Factory;
using Newtonsoft.Json;
using ZondervanLibrary.SharedLibrary.Binding;
using Sushi = ZondervanLibrary.Harvester.Core.Repository.Counter.Sushi_4_1;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public class SushiCounterRepository : RepositoryBase, ICounterRepository
    {
        private readonly SushiCounterRepositoryArguments _arguments;
        private readonly ISushiServiceInterfaceClient _client;
        private readonly String _releaseVersion;

        public SushiCounterRepository(SushiCounterRepositoryArguments arguments, IFactory<ISushiServiceInterfaceClient, EndpointAddress> clientFactory)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(clientFactory != null);

            Name = arguments.Name;
            _arguments = arguments;
            _client = clientFactory.CreateInstance(new EndpointAddress(_arguments.Url));
            _releaseVersion = arguments.ReleaseVersion == ReleaseVersion.R4 ? "4" : "3";
        }

        private IReportItem[] GetResponse(DateTime runDate, String reportName)
        {
            IReportRequest reportRequest = _client.GenerateReportRequest(runDate, reportName, _arguments, _releaseVersion);

            ICounterReportResponse response;

            try
            {
                response = _client.GetReport(reportRequest);
            }
            catch (FaultException exception)
            {
                throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidHost, this, String.Format(RepositoryExceptionMessage.UnrecognizedException, _arguments.Url), exception);
            }
            catch (EndpointNotFoundException exception)
            {
                throw new RepositoryIOException(IOExceptionCategory.NetworkUnavailable, RepositoryExceptionMessage.NetworkUnavailableWait, this, String.Format(RepositoryExceptionMessage.NetworkUnavailable_1, _arguments.Url), exception);
            }
            catch (TimeoutException exception)
            {
                throw new RepositoryIOException(IOExceptionCategory.TimedOut, RepositoryExceptionMessage.TimedOutWait, this, RepositoryExceptionMessage.Timeout, exception);
            }
            catch (ServerTooBusyException exception)
            {
                throw new RepositoryIOException(IOExceptionCategory.General, oneHour, this, "The server was too busy to handle the request", exception);
            }

            if (response.Exception == null || response.Exception.Length <= 0)
                return response.Report?[0].Customer?[0].ReportItems;
            
            switch (response.Exception[0].Number)
            {
                case 1000:
                    throw new RepositoryIOException(IOExceptionCategory.General, oneHour, this, "The service is not available.");
                case 1010:
                    throw new RepositoryIOException(IOExceptionCategory.General, oneHour, this, "The server was too busy to handle the request.");
                case 1020:
                    throw new RepositoryIOException(IOExceptionCategory.General, oneDay, this, "The client has made too many requests today.");
                case 2000:
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidCredentials, this, "The Requestor ID was not recognized by the server.");
                case 2010:
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.InvalidCredentials, this, "The Customer ID was not recognized by the server.");
                case 3000:
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.NotSupported, this, $"The server does not support {reportRequest.ReportDefinition.Name}");
                case 3010:
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.NotSupported, this, $"The server does not support {reportRequest.ReportDefinition.Name} release version {reportRequest.ReportDefinition.Release}");
                case 3030:
                case 10:
                    return null;
                case 3040:
                    // Partial data returned?
                    break;
                //Proquest
                case 20101:
                    Console.WriteLine("ProQuest did not find any data for {0} release version {1}", reportRequest.ReportDefinition.Name, reportRequest.ReportDefinition.Release);
                    return null;
                //Lexis Nexis
                case 3001:
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.NotSupported, this, "The Lexis Nexis does not support CR2 reports since it is not a consortium account");
                case 11:
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.NotSupported, this, $"The Lexis Nexis does not support {reportRequest.ReportDefinition.Name}");
                //Highwire
                case 1:
                    Console.WriteLine("HighWire did not find any data for {0} release version {1}", reportRequest.ReportDefinition.Name, reportRequest.ReportDefinition.Release);
                    return null;
                default:
                    throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, RepositoryExceptionMessage.UnrecognizedError);
            }

            return response.Report[0].Customer[0].ReportItems;
        }

        private const int oneDay = 86400;
        private const int oneHour = 3600;

        #region IJournalRepository

        private ItemType Convert(Sushi.DataType dataType)
        {
            switch (dataType)
            {
                case Sushi.DataType.Book:
                    return ItemType.Book;
                case Sushi.DataType.Collection:
                    return ItemType.Collection;
                case Sushi.DataType.Database:
                    return ItemType.Database;
                case Sushi.DataType.Journal:
                    return ItemType.Journal;
                case Sushi.DataType.Multimedia:
                    return ItemType.Multimedia;
                case Sushi.DataType.Platform:
                    return ItemType.Database;
            }

            throw new ArgumentException("dataType");
        }

        private IdentifierType Convert(Sushi.IdentifierType identifierType)
        {
            switch (identifierType)
            {
                case Sushi.IdentifierType.DOI:
                    return IdentifierType.Doi;
                case Sushi.IdentifierType.ISBN:
                    return IdentifierType.Isbn;
                case Sushi.IdentifierType.Online_ISBN:
                    return IdentifierType.OnlineIsbn;
                case Sushi.IdentifierType.Online_ISSN:
                    return IdentifierType.OnlineIssn;
                case Sushi.IdentifierType.Print_ISBN:
                    return IdentifierType.PrintIsbn;
                case Sushi.IdentifierType.Print_ISSN:
                    return IdentifierType.PrintIssn;
                case Sushi.IdentifierType.Proprietary:
                    return IdentifierType.Proprietary;
            }

            throw new ArgumentException("identifierType");
        }

        private CounterIdentifier Convert(Sushi.Identifier identifier, String itemPlatform, Sushi.DataType dataType)
        {
            IdentifierType resultType;
            String resultValue = identifier.Value;

            switch (identifier.Type)
            {
                case Sushi.IdentifierType.DOI:
                    resultType = IdentifierType.Doi;
                    break;
                case Sushi.IdentifierType.ISBN:
                    resultType = IdentifierType.Isbn;
                    resultValue = resultValue.Replace("-", "");
                    break;
                case Sushi.IdentifierType.Online_ISBN:
                    resultType = IdentifierType.OnlineIsbn;
                    resultValue = resultValue.Replace("-", "");
                    break;
                case Sushi.IdentifierType.Online_ISSN:
                    if (resultValue.Length > 9)
                    {
                        Regex regular = new Regex("[0-9]{10}|[0-9]{13}");
                        if (!regular.IsMatch(resultValue))
                            return null;
                        resultType = IdentifierType.PrintIsbn;
                    }
                    else
                    {
                        Regex regular = new Regex("[0-9]{4}-?[0-9]{4}");
                        if (!regular.IsMatch(resultValue))
                            return null;
                        resultType = IdentifierType.OnlineIssn;
                    }
                        break;
                case Sushi.IdentifierType.Print_ISBN:
                    resultType = IdentifierType.PrintIsbn;
                    resultValue = resultValue.Replace("-", "");
                    break;
                case Sushi.IdentifierType.Print_ISSN:
                    if (resultValue.Length > 9)
                    {
                        Regex regular = new Regex("[0-9]{10}|[0-9]{13}");
                        if (!regular.IsMatch(resultValue))
                            return null;
                        resultType = IdentifierType.PrintIsbn;
                    }
                    else
                    {
                        Regex regularx = new Regex("[0-9]{4}-?[0-9]{4}");
                        if (!regularx.IsMatch(resultValue))
                            return null;
                        resultType = IdentifierType.PrintIssn;
                    }
                    break;
                case Sushi.IdentifierType.Proprietary:
                    resultType = IdentifierType.Proprietary;
                    if (dataType != Sushi.DataType.Platform && dataType != Sushi.DataType.Database)
                    {
                        resultValue = $"{itemPlatform}:{identifier.Value}";
                    }
                    break;
                default:
                    throw new ArgumentException("identifier");
            }

            return new CounterIdentifier { IdentifierType = resultType, IdentifierValue = resultValue };
        }

        private MetricType Convert(Sushi.MetricType metricType)
        {
            switch (metricType)
            {
                case Sushi.MetricType.@abstract:
                    return MetricType.AbstractRequests;
                case Sushi.MetricType.audio:
                    return MetricType.AudioRequests;
                case Sushi.MetricType.data_set:
                    return MetricType.DataSetRequests;
                case Sushi.MetricType.ft_epub:
                    return MetricType.FullTextEpubRequests;
                case Sushi.MetricType.ft_html:
                    return MetricType.FullTextHtmlRequests;
                case Sushi.MetricType.ft_html_mobile:
                    return MetricType.FullTextHtmlMobileRequests;
                case Sushi.MetricType.ft_pdf:
                    return MetricType.FullTextPdfRequests;
                case Sushi.MetricType.ft_pdf_mobile:
                    return MetricType.FullTextPdfMobileRequests;
                case Sushi.MetricType.ft_ps:
                    return MetricType.FullTextPsRequests;
                case Sushi.MetricType.ft_ps_mobile:
                    return MetricType.FullTextPsMobileRequests;
                case Sushi.MetricType.ft_total:
                    return MetricType.FullTextTotalRequests;
                case Sushi.MetricType.image:
                    return MetricType.ImageRequests;
                case Sushi.MetricType.multimedia:
                    return MetricType.MultimediaRequests;
                case Sushi.MetricType.no_license:
                    return MetricType.NoLicenseDenials;
                case Sushi.MetricType.podcast:
                    return MetricType.PodcastRequests;
                case Sushi.MetricType.record_view:
                    return MetricType.RecordViews;
                case Sushi.MetricType.reference:
                    return MetricType.ReferenceRequests;
                case Sushi.MetricType.result_click:
                    return MetricType.ResultClicks;
                case Sushi.MetricType.search_fed:
                    return MetricType.FederatedSearches;
                case Sushi.MetricType.search_reg:
                    return MetricType.RegularSearches;
                case Sushi.MetricType.sectioned_html:
                    return MetricType.SectionedHtml;
                case Sushi.MetricType.toc:
                    return MetricType.TocRequests;
                case Sushi.MetricType.turnaway:
                    return MetricType.TurnawayDenials;
                case Sushi.MetricType.video:
                    return MetricType.VideoRequests;
                case Sushi.MetricType.other:
                    return MetricType.DataSetRequests;
            }

            throw new ArgumentException(nameof(metricType));
        }

        public IEnumerable<CounterRecord> RequestRecords(DateTime runDate, CounterReport report)
        {
            if (_arguments.JsonRepository != null)
            {
                using (IDirectoryRepository localJsonRepo = RepositoryFactory.CreateDirectoryRepository(_arguments.JsonRepository))
                {
                    List<DirectoryObjectMetadata> jsonFiles = localJsonRepo.ListFiles().ToList();

                    if (jsonFiles.Any(x => x.Name == $"{Name} {runDate:yyyy-MM-dd} {report}.json"))
                    {
                        DirectoryObjectMetadata directoryObjectMetadata = jsonFiles.First(x => x.Name == $"{Name} {runDate:yyyy-MM-dd} {report}.json");
                        using (Stream openFile = localJsonRepo.OpenFile(directoryObjectMetadata.Name))
                        {
                            string jsonString = new StreamReader(openFile).ReadToEnd();
                            foreach (var record in JsonConvert.DeserializeObject<IEnumerable<CounterRecord>>(jsonString))
                            {
                                yield return record;
                            }
                        }

                        yield break;
                    }
                }
            }

            LogMessage($"Requesting {report.ToString()}");
            var reportItems = GetResponse(runDate, report.ToString());

            if (reportItems == null)
                yield break;

            foreach (IReportItem reportItem in reportItems)
            {
                if (reportItem.ItemDataType == Sushi.DataType.Platform && string.IsNullOrWhiteSpace(reportItem.ItemName))
                    reportItem.ItemName = reportItem.ItemPublisher;

                yield return new CounterRecord
                {
                    ItemName = reportItem.ItemName,
                    ItemPlatform = reportItem.ItemPlatform,
                    ItemType = Convert(reportItem.ItemDataType),
                    RunDate = runDate,
                    Identifiers = reportItem.ItemIdentifier.Bind(a => a.Select(i => Convert(i, reportItem.ItemPlatform, reportItem.ItemDataType)).Where(i => i != null).ToArray(), new CounterIdentifier[] { }),
                    Metrics = reportItem.ItemPerformance.SelectMany(m => m.Instance).Select(i => new CounterMetric {MetricType = Convert(i.MetricType), MetricValue = Int32.Parse(i.Count)}).ToArray()
                };
            }
        }

        public Action<String> LogMessage { get; set; }

        #endregion IJournalRepository

        #region IRepository

        /// <inheritdoc/>
        public override string ConnectionString => $"SUSHI | Url = {_arguments.Url}, RequestorID = {_arguments.RequestorID}, CustomerID = {_arguments.CustomerID}";

        /// <inheritdoc/>
        public override void Dispose()
        {
            if (_client != null && _client.State != CommunicationState.Faulted)
                _client.Close();
        }

        #endregion IRepository

        public IEnumerable<CounterReport> AvailableReports => _arguments.AvailableReports;
    }
}