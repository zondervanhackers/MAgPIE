using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using HtmlAgilityPack;
using SimpleBrowser;
using ZondervanLibrary.Harvester.Core.Exceptions;
using ZondervanLibrary.SharedLibrary.Binding;
using ZondervanLibrary.SharedLibrary.Factory;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public class EbscoHostCounterRepository : RepositoryBase, ICounterRepository
    {
        private readonly EbscoHostCounterRepositoryArguments arguments;
        private readonly SushiCounterRepository sushiRepository;
        private List<CounterRecord> manualRecordCache;
        private DateTime manualRecordCacheDate;
        private static readonly Dictionary<string, string> identifierCorrections = new Dictionary<string, string>
        {
            { "10631799", "01631799" },
            { "10540142", "15410412" },
            { "00219675", "00219649" },
            { "13685456", "13665456" },
            { "02653789", "02653788" },
            { "33183324", "15322882" },
            { "08888513", "08883513" },
            { "1435224", "14352249" },
            { "01485753", "00095753" },
            { "09845379", "10846832" },
            { "01971262", "10971262" },
            { "10457233", "19312431" },
            { "10828913", "10828931" },
            { "00328296", "0032633X" },
            { "07334278", "07334273" },
            { "01631779", "01631799" },
            { "0740065", "07400659" },
            { "09843346", "08943346" },
            { "10471885", "10774610" },
            { "15589578", "15589587" },
            { "1010144X", "1040144X" },
            { "00981921", "00061921" },
            { "03798913", "07398913" },
            { "07399657", "07398913" },
            { "21569491", "21569401" },
            { "03600080", "03608808" },
            { "03612442", "03642097" },
            { "10851775", "87502011" },
            { "01594221", "10594221" },
            { "13752434", "08845379" },
            { "17312148", "19325770" },
            { "00112196", "00112186" },
            { "9780874806", "9780874806489" },
            { "9783525512249", "9781438268156" },
            { "9783868350", "16161963" },
            { "9780387798", "9780387798219" },
            { "9780226803570", "13698486" },
            { "978521519410", "14680378" },
            { "9780802863216", "9780802863218" },
            { "1566396689156", "9781566396691" },
            { "9783525538096", "9783525530894" },
            { "3525358300", "9783525358344" },
            { "1888157X", "0888157X" },
            { "9782730214", "9782730216074" },
            { "0801030275", "9780801030277" },
            { "04732550", "10656219" },
            { "9780838752", "9780838752524" },
            { "9780838753", "9781611480986" },
            { "3799069", "03799069" },
            { "01470641", "01476041" },
            { "15354738", "15354768" },
        };

        public EbscoHostCounterRepository(EbscoHostCounterRepositoryArguments arguments, IFactory<SushiCounterRepository, SushiCounterRepositoryArguments> repositoryFactory)
        {
            Contract.Requires(arguments != null);
            Contract.Requires(arguments.Name != null);
            Contract.Requires(repositoryFactory != null);

            Name = arguments.Name;
            this.arguments = arguments;
            sushiRepository = repositoryFactory.CreateInstance(arguments.Sushi);
        }

        /// <inheritdoc/>
        public IEnumerable<CounterReport> AvailableReports => arguments.Sushi.AvailableReports;

        public override string ConnectionString => $"SUSHI | Url = {arguments.Sushi.Url}, RequestorID = {arguments.Sushi.RequestorID}, CustomerID = {arguments.Sushi.CustomerID}) (EbscoHost | Url = {"http://eadmin.ebscohost.com/eadmin/login.aspx"}, Username = {arguments.Username}";

        public Action<string> LogMessage { get; set; }

        /// <inheritdoc/>
        /// <remarks>
        ///     <para>
        ///           Ebscohost puts us into a tricky spot.  Thus Sushi protocol has no method of
        ///           determining which database a journal's usage records are associated with.
        ///           EBSCOhost's online proprietary report does have such a means, but it is very slow.
        ///           Our solution here is to request all journal/ebook information at one time and cache
        ///           it across multiple calls to RequestRecords.  Thus we can massage the data into the
        ///           correct format for the requested CounterReport without a significant performance
        ///           penalty.
        ///     </para>
        /// </remarks>
        public IEnumerable<CounterRecord> RequestRecords(DateTime runDate, CounterReport report)
        {
            switch (report)
            {
                case CounterReport.JR1:
                case CounterReport.JR2:
                case CounterReport.JR3:
                case CounterReport.BR1:
                case CounterReport.BR3:
                    if (manualRecordCacheDate != runDate)
                    {
                        manualRecordCache = RequestManualRecords(runDate).ToList();
                        manualRecordCacheDate = runDate;
                    }

                    switch (report)
                    {
                        case CounterReport.JR1:
                            return FilterCache(c => c.ItemType == ItemType.Journal && c.Metrics.Any(m => m.MetricType == MetricType.FullTextTotalRequests && m.MetricValue > 0),
                                m => m.MetricType.EqualsAny(MetricType.FullTextHtmlRequests, MetricType.FullTextPdfRequests, MetricType.FullTextTotalRequests) && m.MetricValue > 0);
                        
                        case CounterReport.JR2:
                            return FilterCache(c => c.ItemType == ItemType.Journal && c.Metrics.Any(m => m.MetricType == MetricType.TurnawayDenials && m.MetricValue > 0),
                                m => m.MetricType == MetricType.TurnawayDenials && m.MetricValue > 0);

                        case CounterReport.JR3:
                            return FilterCache(c => c.ItemType == ItemType.Journal && c.Metrics.Any(m => m.MetricType.EqualsAny(MetricType.FullTextTotalRequests, MetricType.AbstractRequests, MetricType.AudioRequests, MetricType.ImageRequests, MetricType.VideoRequests, MetricType.TurnawayDenials) && m.MetricValue > 0),
                                m => m.MetricType.EqualsAny(MetricType.FullTextHtmlRequests, MetricType.FullTextPdfRequests, MetricType.FullTextTotalRequests, MetricType.AbstractRequests, MetricType.AudioRequests, MetricType.ImageRequests, MetricType.VideoRequests, MetricType.TurnawayDenials) && m.MetricValue > 0);

                        case CounterReport.BR1:
                            return FilterCache(c => c.ItemType == ItemType.Book && c.Metrics.Any(m => m.MetricType == MetricType.FullTextEpubRequests && m.MetricValue > 0),
                                m => m.MetricType.EqualsAny(MetricType.FullTextHtmlRequests, MetricType.FullTextPdfRequests, MetricType.FullTextTotalRequests, MetricType.FullTextEpubRequests) && m.MetricValue > 0);

                        case CounterReport.BR3:
                            return FilterCache(c => c.ItemType == ItemType.Book && c.Metrics.Any(m => m.MetricType == MetricType.TurnawayDenials && m.MetricValue > 0),
                                m => m.MetricType == MetricType.TurnawayDenials && m.MetricValue > 0);

                        default:
                            throw new NotImplementedException("Report type not handled in code: " + report);
                    }

                case CounterReport.DB1:
                    return sushiRepository.RequestRecords(runDate, report);
                default:
                    throw new RepositoryConfigurationException(ConfigurationExceptionCategory.NotSupported, this, "The server does not support this report.");
            }
        }

        public override void Dispose()
        {
            sushiRepository?.Dispose();
        }

        /// <summary>
        /// Helper function to avoid repetitive code for filtering the manually requested cache.
        /// </summary>
        /// <param name="recordFilter">recordFilter</param>
        /// <param name="metricFilter">metricFilter</param>
        /// <returns></returns>
        private IEnumerable<CounterRecord> FilterCache(Func<CounterRecord, bool> recordFilter, Func<CounterMetric, bool> metricFilter)
        {
            return manualRecordCache.Where(recordFilter).Select(c => new CounterRecord
            {
                ItemName = c.ItemName,
                ItemPlatform = c.ItemPlatform,
                ItemType = c.ItemType,
                RunDate = c.RunDate,
                Identifiers = c.Identifiers,
                Metrics = c.Metrics.Where(metricFilter).ToArray()
            });
        }

        /// <summary>
        /// Screen scrape the EBSCOhost website to collect usage statistics.
        /// </summary>
        /// <param name="runDate">runDate</param>
        /// <remarks>
        ///     <para>We use SimpleBrowser here as it automatically takes care of passing the .NET state information along with the forms.</para>
        /// </remarks>
        private IEnumerable<CounterRecord> RequestManualRecords(DateTime runDate)
        {
            return WebCrawlMethod(runDate);
        }

        private IEnumerable<CounterRecord> FiddlerGeneratedCode(DateTime runDate)
        {
            MakeRequests();
            return null;
        }

        private void MakeRequests()
        {
            if (Request_eadmin_ebscohost_com(out HttpWebResponse response))
            {
                response.Close();
            }

            if (Request_eadmin_ebscohost_com_login_aspx(out response))
            {
                response.Close();
            }

            if (Request_eadmin_ebscohost_com_login_aspx_1(out response))
            {
                response.Close();
            }

            if (Request_eadmin_ebscohost_com_gForm_aspx(out response))
            {
                response.Close();
            }

            if (Request_admin_ebi_ebscohost_com(out response))
            {
                response.Close();
            }

            if (Request_admin_ebi_ebscohost_com_reports(out response))
            {
                response.Close();
            }
        }

        private bool Request_eadmin_ebscohost_com(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eadmin.ebscohost.com/");

                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                response?.Close();
                return false;
            }

            return true;
        }

        private bool Request_eadmin_ebscohost_com_login_aspx(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f");

                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                response?.Close();
                return false;
            }

            response.Close();
            return true;
        }

        private bool Request_eadmin_ebscohost_com_login_aspx_1(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f");

                request.KeepAlive = true;
                request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
                request.Headers.Add("Origin", @"http://eadmin.ebscohost.com");
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.ContentType = "application/x-www-form-urlencoded";
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Referer = "http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=amy4mybsm0025vzzpk4vx4x0");

                request.Method = "POST";
                request.ServicePoint.Expect100Continue = false;

                string body = @"__VIEWSTATE=%2FwEPDwULLTE1NzE4NTQ5OTQPFgIeE1ZhbGlkYXRlUmVxdWVzdE1vZGUCARYCAgEPZBYKAgEPFgIeBGhyZWYFJmh0dHA6Ly9lYWRtaW4uZWJzY29ob3N0LmNvbS9sb2dpbi5hc3B4ZAIDDxYCHwEFGGh0dHBzOi8vd3d3LmVic2NvbmV0LmNvbWQCBQ8WAh8BBShodHRwczovL2VjbS5lYnNjb2hvc3QuY29tL1VzZXIvTG9naW5Gb3JtZAIHDxYCHwEFIWh0dHA6Ly93d3cubGlicmFyeWF3YXJlLmNvbS9sb2dpbmQCFQ9kFgpmDw8WAh4HVmlzaWJsZWhkZAICDw8WAh4LTmF2aWdhdGVVcmwFPWh0dHA6Ly9zdXBwb3J0LmVic2NvaG9zdC5jb20vQ3VzdFN1cHBvcnQvQ3VzdG9tZXIvU2VhcmNoLmFzcHhkZAIEDw8WAh8DBS9odHRwOi8vc3VwcG9ydC5lYnNjb2hvc3QuY29tL2Vob3N0L3ByaXZhY3kuaHRtbGRkAgYPDxYCHwMFLWh0dHA6Ly9zdXBwb3J0LmVic2NvaG9zdC5jb20vZWhvc3QvdGVybXMuaHRtbGRkAggPDxYCHwMFN2h0dHA6Ly9zdXBwb3J0LmVic2NvaG9zdC5jb20vZWhvc3QvdGVybXMuaHRtbCNjb3B5cmlnaHRkZGQffen%2BM0LLTN%2FvXVCr7cMEx0ecBg%3D%3D&__VIEWSTATEGENERATOR=C2EE9ABB&__EVENTVALIDATION=%2FwEdAAMAnYrRlkshU%2F1NGUtTaetwR1LBKX1P1xh290RQyTesRVwK8%2F1gnn25OldlRNyIedkcyZ%2FziwdW29jCde%2B03TOvXj2yAg%3D%3D&UserName=[Username]&Password=L0w3%21%21H%40%24n3s";
                byte[] postBytes = Encoding.UTF8.GetBytes(body);
                request.ContentLength = postBytes.Length;
                Stream stream = request.GetRequestStream();
                stream.Write(postBytes, 0, postBytes.Length);
                stream.Close();

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                response?.Close();
                return false;
            }

            response.Close();
            return true;
        }

        private bool Request_eadmin_ebscohost_com_gForm_aspx(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://eadmin.ebscohost.com/Profiles/CustomizeServiceSearchingForm.aspx");

                request.KeepAlive = true;
                request.Headers.Set(HttpRequestHeader.CacheControl, "max-age=0");
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Referer = "http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                request.Headers.Set(HttpRequestHeader.Cookie, @"ASP.NET_SessionId=amy4mybsm0025vzzpk4vx4x0; eadmin=F95EBE06BD1420F2EC7240B0528A522274925ADC77FD2228C827EB4E0FD23336366CD9B93C918DDD2E9C4CE7BB44199FFBB19284FFF0B2A5EB982DC560303591D4E3C9EC9E4B2ADCDC4ED947307B3D3B640D3F4A687980BBEF248DB59EDBB88D07CCB31EF0D4CB07ED3A543EE37A40B382B9020158C3EE60AC5E45B085DCC61790343F59B5EE3B7B26207A211949E5507FADA4A02BD4BEE1E74BBA394EB71D8FE6FE8D26");

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                response?.Close();
                return false;
            }

            return true;
        }

        private bool Request_admin_ebi_ebscohost_com(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://admin.ebi.ebscohost.com/reports");

                request.KeepAlive = true;
                request.Headers.Add("Upgrade-Insecure-Requests", @"1");
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.Referer = "http://eadmin.ebscohost.com/Profiles/CustomizeServiceSearchingForm.aspx";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                request.Headers.Set(HttpRequestHeader.Cookie, @"eadmin=94934F1933D7BE2B0CF8BDC4F3F4BBCA4A678C774D7D742BF5794C09ADF46961E8188D21BE72423F45E0A234DDCB40886F7ACA18FA84DE9A4EA505F0B2478E185C78DFF85E180D6FBF0C306E86ED7CF7A661689536B8134CBAEA0C774B165E3B4957617B86378130987B4B2FB473947FF36A81C83279994584E1B60B2C31C8044C8EF7945248FDD4E79767E5D835DA2C3ED0EA4C9795DEBEAB8320160E305526E9E22C7CDCED0136208426F853C5D2A840F94085");

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                response?.Close();
                return false;
            }

            response.Close();
            return true;
        }

        private bool Request_admin_ebi_ebscohost_com_reports(out HttpWebResponse response)
        {
            response = null;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://admin.ebscohost.com/reports");

                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36";
                request.Accept = "image/webp,image/apng,image/*,*/*;q=0.8";
                request.Referer = "http://admin.ebscohost.com/reports";
                request.Headers.Set(HttpRequestHeader.AcceptEncoding, "gzip, deflate");
                request.Headers.Set(HttpRequestHeader.AcceptLanguage, "en-US,en;q=0.8");
                request.Headers.Set(HttpRequestHeader.Cookie, @"eadmin=94934F1933D7BE2B0CF8BDC4F3F4BBCA4A678C774D7D742BF5794C09ADF46961E8188D21BE72423F45E0A234DDCB40886F7ACA18FA84DE9A4EA505F0B2478E185C78DFF85E180D6FBF0C306E86ED7CF7A661689536B8134CBAEA0C774B165E3B4957617B86378130987B4B2FB473947FF36A81C83279994584E1B60B2C31C8044C8EF7945248FDD4E79767E5D835DA2C3ED0EA4C9795DEBEAB8320160E305526E9E22C7CDCED0136208426F853C5D2A840F94085; ASP.NET_SessionId=2kxykodsz2zkwjbnl1hc3cfl");

                response = (HttpWebResponse)request.GetResponse();
            }
            catch (WebException e)
            {
                if (e.Status == WebExceptionStatus.ProtocolError) response = (HttpWebResponse)e.Response;
                else return false;
            }
            catch (Exception)
            {
                response?.Close();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Do a quick run of JR1 and DB1 in order to get better sanitized name if available and a 
        /// more complete set of identifiers (JR1/DB1 usually contain both print and online ISSN/ISBN
        /// whereas the custom report only returns one identifier).
        /// </summary>
        /// <param name="runDate">runDate</param>
        private Dictionary<string, CounterRecord> CreateCounterLookup(DateTime runDate)
        {
            IEnumerable<CounterRecord> jr1Records = sushiRepository.RequestRecords(runDate, CounterReport.JR1);
            IEnumerable<CounterRecord> db1Records = sushiRepository.RequestRecords(runDate, CounterReport.BR1);

            // We don't care here that we enumerate the enumerable with ToList() as all the elements will
            // be referenced in memory by the dictionary anyways.
            List<CounterRecord> allRecords = jr1Records.Union(db1Records).ToList();

            Dictionary<String, CounterRecord> counterLookup = new Dictionary<string, CounterRecord>(allRecords.Count);

            foreach (CounterRecord record in allRecords)
            {
                record.Identifiers.Where(i => i.IdentifierType.EqualsAny(IdentifierType.OnlineIsbn, IdentifierType.OnlineIssn, IdentifierType.PrintIsbn, IdentifierType.PrintIssn))
                                  .Select(c => (c.IdentifierValue.Length > 9) ? c.IdentifierValue.Replace("-", "") : c.IdentifierValue)
                                  .Where(i => !counterLookup.ContainsKey(i))
                                  .Distinct()
                                  .ToList()
                                  .ForEach(s =>
                                  {
                                      counterLookup.Add(s, record);
                                  });
            }

            return counterLookup;
        }

        private IEnumerable<CounterRecord> WebCrawlMethod(DateTime runDate)
        {
            EbscoWebCrawler crawler = new EbscoWebCrawler("Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/54.0.2840.87 Safari/537.36",  LogMessage);

            HttpWebResponse response;
            Request_eadmin_ebscohost_com_login_aspx(out response);
            Request_eadmin_ebscohost_com_login_aspx_1(out response);

            //crawler.MakeRequest("http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f", "GET", null, null, new[] { "" });


            String viewState = "%2FwEPDwULLTE1NzE4NTQ5OTQPFgIeE1ZhbGlkYXRlUmVxdWVzdE1vZGUCARYCAgEPZBYKAgEPFgIeBGhyZWYFJmh0dHA6Ly9lYWRtaW4uZWJzY29ob3N0LmNvbS9sb2dpbi5hc3B4ZAIDDxYCHwEFGGh0dHBzOi8vd3d3LmVic2NvbmV0LmNvbWQCBQ8WAh8BBShodHRwczovL2VjbS5lYnNjb2hvc3QuY29tL1VzZXIvTG9naW5Gb3JtZAIHDxYCHwEFIWh0dHA6Ly93d3cubGlicmFyeWF3YXJlLmNvbS9sb2dpbmQCFQ9kFgpmDw8WAh4HVmlzaWJsZWhkZAICDw8WAh4LTmF2aWdhdGVVcmwFPWh0dHA6Ly9zdXBwb3J0LmVic2NvaG9zdC5jb20vQ3VzdFN1cHBvcnQvQ3VzdG9tZXIvU2VhcmNoLmFzcHhkZAIEDw8WAh8DBS9odHRwOi8vc3VwcG9ydC5lYnNjb2hvc3QuY29tL2Vob3N0L3ByaXZhY3kuaHRtbGRkAgYPDxYCHwMFLWh0dHA6Ly9zdXBwb3J0LmVic2NvaG9zdC5jb20vZWhvc3QvdGVybXMuaHRtbGRkAggPDxYCHwMFN2h0dHA6Ly9zdXBwb3J0LmVic2NvaG9zdC5jb20vZWhvc3QvdGVybXMuaHRtbCNjb3B5cmlnaHRkZGQffen%2BM0LLTN%2FvXVCr7cMEx0ecBg%3D%3D";//crawler.FindValueById("__VIEWSTATE");
            String viewStateGenerator = "C2EE9ABB";//crawler.FindValueById("__VIEWSTATEGENERATOR");
            String eventValidation = "%2FwEdAAMAnYrRlkshU%2F1NGUtTaetwR1LBKX1P1xh290RQyTesRVwK8%2F1gnn25OldlRNyIedkcyZ%2FziwdW29jCde%2B03TOvXj2yAg%3D%3D";//crawler.FindValueById("__EVENTVALIDATION");
            String uploadValues = string.Format(
                "__VIEWSTATE={0}&__VIEWSTATEGENERATOR={1}&__EVENTVALIDATION={2}&UserName={3}&Password={4}",
                Uri.EscapeDataString(viewState), Uri.EscapeDataString(viewStateGenerator), Uri.EscapeDataString(eventValidation), Uri.EscapeDataString(arguments.Username), Uri.EscapeDataString(arguments.Password));

            crawler.MakeRequest("http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f", "POST", uploadValues, "http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f", new[] { "Upgrade-Insecure-Requests: 1", "Keep-Alive: true" });
            
            LogMessage("Logged In");

            crawler.MakeRequest("http://admin.ebscohost.com/reports", "GET", "", "", new[] { "Upgrade-Insecure-Requests: 1", "Keep-Alive: true" });
            crawler.MakeRequest("http://eadmin.ebscohost.com/EAdmin/Reports/SelectReportsForm.aspx", "GET", "", "", new[] { "Upgrade-Insecure-Requests: 1", "Keep-Alive: true" });

            String postData = "__EVENTTARGET=" + WebUtility.UrlEncode("reportType$3") + "&__EVENTARGUMENT=&__LASTFOCUS="
                     + "&__VIEWSTATE=" + WebUtility.UrlEncode(crawler.FindValueById("__VIEWSTATE"))
                     + "&__VIEWSTATEGENERATOR=" + WebUtility.UrlEncode(crawler.FindValueById("__VIEWSTATEGENERATOR"))
                     + "&__EVENTVALIDATION=" + WebUtility.UrlEncode(crawler.FindValueById("__EVENTVALIDATION"))
                     + "&DataChanged=&ConfirmSave=&HyperLinkUrl=&ClearState=&AdminGridKey=&AdminGridCollumn=&AdminGridValue=&AdminGridTitleCtr=&AdminGridTitleCtrValue=&AddedAction=&BrowseButton=&AdminGridUpDownButton=&AdminGridCurrentPage=&ConfirmCancel=&ConfirmRedirect=&AdditionPopupMessage="
                     + "&reportType=" + "title"
                     + "&scope=" + "consortium"
                     + "&scopeCustId=" + WebUtility.UrlEncode(arguments.Username)
                     + "&uf1=" + "alluse"
                     + "&uf1Val=Academic&detail=site"
                     + "&timePeriodFromMonth=" + runDate.Month + "&timePeriodFromYear=" + runDate.Year
                     + "&timePeriodToMonth=" + runDate.Month + "&timePeriodToYear=" + runDate.Year
                     + "&displayBy=Month&include=n"
                     + "&fields=SessCount&fields=SessDuration&fields=Searches&fields=FullText&fields=PdfFTCount&fields=HtmlFTCount&fields=ImageCount&fields=AbstractCount&fields=SmartLinkCount&fields=CustomLinkCount"
                     + "&sort=default_sort_order"
                     + "&reportFormat=" + "html"
                     + "&pageLines=" + "1000"
                     + "&uf2=alluse"
                     + "&database=&intface=&collection=&epConsSite=&whoami=session&hideUf=";
            crawler.MakeRequest("http://eadmin.ebscohost.com/EAdmin/Reports/selectreportsform.aspx", "POST", postData);

            String databasesHtml = crawler.FindValueByName("databaseDbName", "select");
            var databases = databasesHtml.Split(new[] { @"<option value=""" }, StringSplitOptions.RemoveEmptyEntries).Select(x => new { Name = x.Substring(0, x.IndexOf("\">")), Value = x.Substring(x.IndexOf("\">") + 2) });

            var checkBoxValues = new List<string> 
            { "ISSN", "Title", "Turnaways", "FullText", "PdfFTCount", 
                "HtmlFTCount", "EBookFTCount", /*"EBookDownloadCount", */
                "AudioBookDownloadCount", "ImageCount", "AudioCount", 
                "AbstractCount", "SmartLinkCount", "CustomLinkCount"
            }.Select(s => new { Value = s, Checked = false });

            string eventTarget = WebUtility.UrlEncode(crawler.FindValueById("__EVENTTARGET"));
            string eventArgument = WebUtility.UrlEncode(crawler.FindValueById("__EVENTARGUMENT"));
            string lastFocus = WebUtility.UrlEncode(crawler.FindValueById("__LASTFOCUS"));
            viewState = WebUtility.UrlEncode(crawler.FindValueById("__VIEWSTATE"));
            viewStateGenerator = WebUtility.UrlEncode(crawler.FindValueById("__VIEWSTATEGENERATOR"));
            eventValidation = WebUtility.UrlEncode(crawler.FindValueById("__EVENTVALIDATION"));

            string databasePostString = 
                      "__EVENTTARGET={0}&__EVENTARGUMENT={1}&__LASTFOCUS={2}&__VIEWSTATE={3}&__VIEWSTATEGENERATOR={4}&__EVENTVALIDATION={5}&DataChanged=&ConfirmSave=&HyperLinkUrl=&ClearState=&AdminGridKey=&AdminGridCollumn=&AdminGridValue=&AdminGridTitleCtr="
                    + "&AdminGridTitleCtrValue=&AddedAction=&BrowseButton=&AdminGridUpDownButton=&AdminGridCurrentPage=&ConfirmCancel=&ConfirmRedirect=&AdditionPopupMessage=&reportType=title"
                    + "&scope=consortium&scopeCustId={6}&database=list&databaseDbName={7}&timePeriodFromMonth={8}&timePeriodFromYear={9}&timePeriodToMonth={8}&timePeriodToYear={9}"
                    + "&collection=&fields=ISSN&fields=Title&fields=Turnaways&fields=FullText&fields=PdfFTCount&fields=HtmlFTCount&fields=EBookFTCount&fields=EBookDownloadCount&fields=AudioBookDownloadCount"
                    + "&fields=ImageCount&fields=AudioCount&fields=AbstractCount&fields=SmartLinkCount&fields=CustomLinkCount&sort=default_sort_order&reportFormat=html&pageLines=500"
                    + "&showReportButton=Show+Report&uf1=alluse&uf2=alluse&detail=site&intface=&include=n&epConsSite=&whoami=title&hideUf=";
            foreach (var database in databases)
            {
                postData = string.Format(databasePostString, eventTarget, eventArgument, lastFocus, viewState, viewStateGenerator, eventValidation, arguments.Username, database.Name, runDate.Month, runDate.Year);

                crawler.MakeRequest("http://eadmin.ebscohost.com/EAdmin/Reports/selectreportsform.aspx", "POST", postData, "http://eadmin.ebscohost.com/EAdmin/Reports/selectreportsform.aspx");
                          
                int indexofPageNumber = crawler.ResponseText.IndexOf("<span id=\"pageNumberDisplay\" style=\"font-size:X-Small;font-style:italic;\">");
                if (indexofPageNumber < 0)
                    throw new IndexOutOfRangeException(crawler.ResponseText);
                string pageNumberString = crawler.ResponseText.Substring(indexofPageNumber, 120);
                string[] pageNumberValues = pageNumberString.Split(new[] { "&nbsp;" }, StringSplitOptions.RemoveEmptyEntries);
                if (pageNumberValues.Length >= 3 && pageNumberValues[3].ElementAt(0) != '1')
                {
                    crawler.MakeRequest("http://eadmin.ebscohost.com/eadmin/reports/viewreportsform.aspx?r=Title+Usage+Report&t=title&l=2000&q=n&fn=", "GET", "", "http://eadmin.ebscohost.com/EAdmin/Reports/selectreportsform.aspx");
                }

                string[] lines = crawler.ResponseText.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                if (lines.Length > 80 && lines[81].Contains("class=\"error\""))
                    continue;

                int records = 0;
                List<String> omittedFields = new List<string> { "Audio", "Smart Link", "Custom Link" };
                
                IEnumerable<String> dataLines = lines.Where((x, i) => x.Contains("<td class=\"DataGrid-ItemStyle-ControlColumn\" align=\"left\"><span id=\"grid_MainDataGrid__ct"));
                HtmlDocument doc = new HtmlDocument();
                foreach (String dataLine in dataLines)
                {
                    string line = dataLine.Split(new[] { "\t" }, StringSplitOptions.RemoveEmptyEntries).First();
                    doc.LoadHtml(line);

                    IEnumerable<HtmlNode> descendants = doc.DocumentNode.Descendants();
                    HtmlNode[] metrics = descendants.Where(x => x.Name == "span" && !omittedFields.Any(o => x.Id.Contains(o))).ToArray();
                    if (metrics.Length < 2)
                        continue;

                    string identifierString = metrics[0].InnerText;

                    // The ERIC database contains this strange record at the bottom???
                    // Skip for now.
                    if (identifierString == "ERICRIE0")
                        continue;

                    // Why Christian Periodical Index? Why?
                    if (identifierString == "Timely")
                        continue;

                    if (metrics[1].InnerText == "EMPTY")
                        continue;

                    // Until EBSCOhost gets their act together.
                    if (identifierCorrections.ContainsKey(identifierString))
                        identifierString = identifierCorrections[identifierString];

                    ItemType itemType;
                    IdentifierType identifierType;

                    //CounterRecord lookupRecord = null;
                    if (identifierString.Length == 10 || identifierString.Length == 13)
                    {
                        itemType = ItemType.Book;
                        identifierType = IdentifierType.OnlineIsbn;

                        identifierString = CounterRecord.SanitizeISBN(identifierString);
                    }
                    else
                    {
                        itemType = ItemType.Journal;
                        identifierType = IdentifierType.OnlineIssn;

                        identifierString = CounterRecord.SanitizeISSN(identifierString);
                    }

                    CounterIdentifier[] identifiers = { new CounterIdentifier { IdentifierType = identifierType, IdentifierValue = identifierString } };

                    MetricType[] metricColumns = new[]
                    {
                        MetricType.TurnawayDenials,
                        MetricType.FullTextTotalRequests,
                        MetricType.FullTextPdfRequests,
                        MetricType.FullTextHtmlRequests,
                        MetricType.FullTextEpubRequests,
                        MetricType.ImageRequests,
                        MetricType.AudioRequests,
                        MetricType.AbstractRequests
                    };
                    records++;
                    yield return new CounterRecord
                    {
                        ItemName = metrics[1].InnerText,
                        ItemPlatform = database.Value.Replace("EBSCO Information Services: ", ""),
                        ItemType = itemType,
                        RunDate = runDate,
                        Identifiers = identifiers,
                        Metrics = metricColumns.Select((c, index) => new CounterMetric { MetricType = c, MetricValue = int.Parse(metrics[index + 2].InnerText) }).ToArray()
                    };
                }

                LogMessage($"{database.Value}({records})");
            }
        }

        private IEnumerable<CounterRecord> SimpleBrowserMethod(DateTime runDate)
        {
            //var counterLookup = CreateCounterLookup(runDate);
            //"__VIEWSTATE={0}&__VIEWSTATEGENERATOR={1}&__EVENTVALIDATION={2}

            Browser browser = new Browser
            {
                UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/61.0.3163.100 Safari/537.36",
                AutoRedirect = false,
                RefererMode = Browser.RefererModes.Origin,
            };

            // Chrome 43.0.2357.130
            browser.Navigate("http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f");
            //if (LastRequestFailed(browser)) throw new OperationException("There was an error loading the page: " + browser.LastWebException.Message);

            HtmlResult result = browser.Find("UserName");
            if (result.Exists)
                result.Value = arguments.Username;
            else
                throw new MissingFieldException("UserName field was not found");

            result = browser.Find("Password");
            if (result.Exists)
                result.Value = arguments.Password;
            else
                throw new MissingFieldException("Password field was not found");

            result = browser.Find("UserName");
            if (result.Exists)
                result.Value = arguments.Username;
            else
                throw new MissingFieldException("UserName field was not found");

            result = browser.Find("Password");
            if (result.Exists)
                result.Value = arguments.Password;
            else
                throw new MissingFieldException("Password field was not found");

            browser.SetHeader("Accept-Language: en-US,en;q=0.8");
            browser.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
            browser.SetHeader("Upgrade-Insecure-Requests: 1");
            browser.SetHeader("Origin: http://eadmin.ebscohost.com");
            browser.SetHeader("Cache-Control: max-age=0");
            browser.AutoRedirect = true;
            browser.Find("Form1").SubmitForm("http://eadmin.ebscohost.com/login.aspx?ReturnUrl=%2f");

            //browser.SetContent("Content-Type");// = "application/x-www-form-urlencoded; charset=UTF-8; text/json; application/json; charset=UTF-8";
            //new List<string> { "__VIEWSTATE", "__VIEWSTATEGENERATOR", "__EVENTVALIDATION" }.ForEach(x => );
           
            //var viewState = browser.Find("__VIEWSTATE").Value;
            //var viewStateGenerator = browser.Find("__VIEWSTATEGENERATOR").Value;
            //var eventValidation = browser.Find("__EVENTVALIDATION").Value;
            //var uploadValues = new NameValueCollection
            //{ 
            //    { "__VIEWSTATE", viewState },
            //    { "__VIEWSTATEGENERATOR", viewStateGenerator }, 
            //    { "__EVENTVALIDATION", eventValidation }, 
            //    { "UserName", arguments.Username }, 
            //    { "Password", Uri.EscapeDataString(arguments.Password) }
            //};
            //browser.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            //browser.SetHeader("Accept-Language: en-US,en;q=0.8");
            //browser.SetHeader("Cache-Control: max-age=0");
            //browser.SetHeader("Origin: http://eadmin.ebscohost.com");
            //browser.SetHeader("Upgrade-Insecure-Requests: 1");
            //browser.Navigate(new Uri("http://eadmin.ebscohost.com/eadmin/login.aspx"), uploadValues, "application/x-www-form-urlencoded");

            //if (LastRequestFailed(browser)) throw new OperationException("There was an error loading the page: " + browser.LastWebException.Message);
            //if (clickResult == ClickResult.SucceededNoNavigation || clickResult == ClickResult.Failed)
            //    throw new OperationException("The Browser was not redirected: Check Username and Password Validity.");

            // Assume that anchor href is the least likely to change of potential identifiers
            //clickResult = browser.Find(ElementType.Anchor, "href", "http://admin.ebscohost.com/reports").Click();
            //if (clickResult == ClickResult.SucceededNoNavigation || clickResult == ClickResult.Failed)
            //    throw new OperationException("The Browser was not redirected: Check Username and Password Validity.");

            // Assume that anchor href is the least likely to change of potential identifiers
            //clickResult = browser.Find(ElementType.Anchor, "href", "http://eadmin.ebscohost.com/EAdmin/Reports/SelectReportsForm.aspx").Click();
            //if (clickResult == ClickResult.SucceededNoNavigation || clickResult == ClickResult.Failed)
            //    throw new OperationException("The Browser was not redirected: Check Username and Password Validity.");

            // Select the "title" radio button and reload the form

            result = browser.Find("UserName");
            if (result.Exists)
                result.Value = arguments.Username;

            result = browser.Find("Password");
            if (result.Exists)
                result.Value = arguments.Password;

            browser.Find(ElementType.RadioButton, FindBy.Value, "title").Checked = true;
            browser.Find("_ctl0").SubmitForm();

            var databases = browser.Select("select[name=databaseDbName] option").Select(o => new { Name = o.Value.Substring(28), Value = o.GetAttribute("value") }).ToArray();

            var checkBoxValues = new List<string> 
            { "ISSN", "Title", "Turnaways", "FullText", "PdfFTCount", 
                "HtmlFTCount", "EBookFTCount", "EBookDownloadCount", 
                "AudioBookDownloadCount", "ImageCount", "AudioCount", 
                "AbstractCount", "SmartLinkCount", "CustomLinkCount" 
            }.Select(s => new { Value = s, Checked = false }).ToArray();

            foreach (var database in databases)
            {
                browser.Find(ElementType.Checkbox, FindBy.Name, "badissn").Checked = false;

                foreach (var pair in checkBoxValues)
                {
                    browser.Find(ElementType.Checkbox, FindBy.Value, pair.Value).Checked = pair.Checked;
                }

                browser.Find(ElementType.SelectBox, FindBy.Name, "timePeriodFromMonth").Value = runDate.Month.ToString();
                browser.Find(ElementType.SelectBox, FindBy.Name, "timePeriodFromYear").Value = runDate.Year.ToString();
                browser.Find(ElementType.SelectBox, FindBy.Name, "timePeriodToMonth").Value = runDate.Month.ToString();
                browser.Find(ElementType.SelectBox, FindBy.Name, "timePeriodToYear").Value = runDate.Year.ToString();

                new List<string> 
                {  "ISSN", "Title", "Turnaways", "FullText", "PdfFTCount", "HtmlFTCount", "EBookFTCount", "ImageCount", "AudioCount", "AbstractCount" 
                }.ForEach(field => { browser.ExtraFormValues.Add("fields", field); });

                browser.Find("reportFormat").Value = "tsv";

                browser.Find("__EVENTTARGET").Value = "saveReportButton";
                browser.Find("__EVENTARGUMENT").Value = "Save Report";

                browser.Select("select[name=databaseDbName]").Value = database.Value;

                browser.Find("_ctl0").SubmitForm();

                string[] lines = browser.CurrentHtml.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                for (int i = 3; i < lines.Length; i++)
                {
                    string[] elements = lines[i].Split('\t');

                    string identifierString = elements[0];

                    // The ERIC database contains this strange record at the bottom???
                    // Skip for now.
                    if (identifierString == "ERICRIE0")
                        continue;

                    // Why Christian Periodical Index? Why?
                    if (identifierString == "Timely")
                        continue;

                    // Until EBSCOhost gets their act together.
                    if (identifierCorrections.ContainsKey(identifierString))
                        identifierString = identifierCorrections[identifierString];

                    CounterIdentifier[] identifiers = null;
                    ItemType itemType = default(ItemType);
                    IdentifierType identifierType = default(IdentifierType);

                    CounterRecord lookupRecord = null;

                    if (identifierString.Length == 10 || identifierString.Length == 13)
                    {
                        itemType = ItemType.Book;
                        identifierType = IdentifierType.OnlineIsbn;

                        // Until EBSCOhost gets their act together.
                        //if (identifierString == "978140295092")
                        //    continue;

                        identifierString = CounterRecord.SanitizeISBN(identifierString);

                        //if (identifierString == null)
                        //    writer.WriteLine(string.Join("\t", database.Name, elements[1], elements[0]));
                        //throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, "Bad ISBN");
                    }
                    else
                    {
                        itemType = ItemType.Journal;
                        identifierType = IdentifierType.OnlineIssn;

                        // Can't identify yet.
                        //if (identifierString == "00208837")
                        //    continue;

                        //if (identifierString == "88990190")
                        //    continue;

                        identifierString = CounterRecord.SanitizeISSN(identifierString);

                        //if (identifierString == null)
                        //    writer.WriteLine(string.Join("\t", database.Name, elements[1], elements[0]));
                        //throw new RepositoryImplementationException(ImplementationExceptionCategory.UnrecognizedException, this, "Bad ISSN");
                    }

                    identifiers = new[] { new CounterIdentifier { IdentifierType = identifierType, IdentifierValue = identifierString } };

                    //if (counterLookup.ContainsKey(identifierString))
                    //{
                    //    lookupRecord = counterLookup[identifierString];
                    //    identifiers = lookupRecord.Identifiers;
                    //}
                    //else
                    //{
                    //    identifiers = new CounterIdentifier[] { new CounterIdentifier { IdentifierType = identifierType, IdentifierValue = identifierString } };
                    //}

                    MetricType[] metricColumns = new[]
                    {
                        MetricType.TurnawayDenials,
                        MetricType.FullTextTotalRequests,
                        MetricType.FullTextPdfRequests,
                        MetricType.FullTextHtmlRequests,
                        MetricType.FullTextEpubRequests,
                        MetricType.ImageRequests,
                        MetricType.AudioRequests,
                        MetricType.AbstractRequests
                    };

                    yield return new CounterRecord()
                    {
                        ItemName = lookupRecord.Bind(r => r.ItemName, elements[1]),
                        ItemPlatform = database.Name,
                        ItemType = itemType,
                        RunDate = runDate,
                        Identifiers = identifiers,
                        Metrics = metricColumns.Select((c, index) => new CounterMetric { MetricType = c, MetricValue = int.Parse(elements[index + 2]) }).ToArray()
                    };
                }

                Console.WriteLine(database);
                browser.NavigateBack();
                //if (LastRequestFailed(browser)) throw new OperationException("There was an error loading the page: " + browser.LastWebException.Message);
            }
        }
    }

    internal class EbscoWebCrawler
    {
        public string ResponseText;
        readonly string userAgent;
        readonly CookieContainer cookies;
        HttpWebResponse currentResponse;
        HttpWebRequest currentRequest;
        private readonly Action<string> LogMessage;

        public EbscoWebCrawler(string useragent, Action<string> logMessage)
        {
            userAgent = useragent;
            LogMessage = logMessage;
            cookies = new CookieContainer();
        }

        public void MakeRequest(string uri, string method = "GET", string uploadValues = "", string referer = "", string[] Headers = null, string Accept = "")
        {
            currentRequest = WebRequest.Create(uri) as HttpWebRequest;
            currentRequest.Host = "eadmin.ebscohost.com";
            currentRequest.UserAgent = userAgent;
            currentRequest.Method = method;
            currentRequest.CookieContainer = cookies;
            currentRequest.AllowAutoRedirect = true;
            currentRequest.KeepAlive = true;

            Headers?.ToList().ForEach(x => currentRequest.Headers.Add(x));

            if (referer != "")
                currentRequest.Referer = referer;
            if (Accept != "")
                currentRequest.Accept = Accept;

            if (uploadValues != "" && method == "POST")
            {
                currentRequest.ContentType = "application/x-www-form-urlencoded; charset=UTF-8; text/json; application/json; charset=UTF-8";
                Byte[] encodedByteData = Encoding.UTF8.GetBytes(uploadValues);
                currentRequest.ContentLength = encodedByteData.Length;

                using (Stream stream = currentRequest.GetRequestStream())
                {
                    stream.Write(encodedByteData, 0, encodedByteData.Length);
                }
            }

            if (method == "GET")
            {
                LogMessage($"{method} Request to {uri}");
            }

            using (currentResponse = (HttpWebResponse) currentRequest.GetResponse())
            {
                cookies.Add(currentResponse.Cookies);

                using (Stream responseStream = currentResponse.GetResponseStream())
                {
                    ResponseText = new StreamReader(responseStream).ReadToEnd();
                }
                currentResponse.Close();
                currentRequest.Abort();
            }
        }

        public string FindValueById(string id)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(ResponseText);
            if (id == "")
                return "";

            HtmlNode foundElement = doc.GetElementbyId(id);
            if (foundElement == null)
                return "";

            if (foundElement.Attributes != null)
                return foundElement.Attributes.First(x => x.Name == "value").Value;

            LogMessage($"value attribute missing in html tag with ID: {foundElement.Id}");
            return "";
        }

        public string FindValueByName(string name, string tag = "")
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(ResponseText);
            HtmlNodeCollection documentNodes = doc.DocumentNode.SelectNodes("//" + tag);
            if (documentNodes == null)
                return "";
            HtmlNode documentNodesWithAttribute = documentNodes.First(x => x.Attributes != null && x.Attributes.Any(y => y.Value == name));
            if (documentNodesWithAttribute == null)
                return "";
            else
                return documentNodesWithAttribute.InnerHtml;
        }
    }
}
