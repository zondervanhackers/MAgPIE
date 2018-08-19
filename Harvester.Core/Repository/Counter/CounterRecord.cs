using System;
using System.Linq;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public class CounterRecord
    {
        public string ItemName { get; set; }

        public string ItemPlatform { get; set; }

        public ItemType ItemType { get; set; }

        public CounterIdentifier[] Identifiers { get; set; }

        public CounterMetric[] Metrics { get; set; }

        public DateTime RunDate { get; set; }

        public static string SanitizeISBN(string isbn)
        {
            switch (isbn.Length)
            {
                case 13:
                    return (isbn.Select((c, i) => ((i % 2) * 2 + 1) * Int32.Parse(c.ToString())).Sum() % 10 == 0) ? isbn : null;
                case 10:
                    return (isbn.Select((c, i) => (i + 1) * ((c == 'X' || c == 'x') ? 10 : Int32.Parse(c.ToString()))).Sum() % 11 == 0) ? $"978{isbn}{ComputeISBN13Checksum(isbn)}" : null;
                default:
                    return null;
            }
        }

        private static Char ComputeISBN13Checksum(string isbn)
        {
            return (char)((10 - isbn.Substring(0, 9).Select((c, i) => ((i % 2) * 2 + 1) * Int32.Parse(c.ToString())).Sum() % 10) % 10 + '0');
        }

        public static string SanitizeISSN(string issn)
        {
            if (!issn.All(c => (c >= '0' && c <= '9') || (c == 'x') || (c == 'X')) || issn == "")
                return null;

            return (issn.Replace("-", "").Select((c, i) => (8 - i) * ((c == 'X' || c == 'x') ? 10
                    : int.Parse(c.ToString()))).Sum() % 11 == 0) ? $"{issn.Substring(0, 4)}-{issn.Substring(4)}"
                : null;
        }
    }

    public enum ItemType
    {
        Journal,
        Database,
        Platform,
        Book,
        Collection,
        Multimedia,
        Vendor,
        Newsletter,
        Newspaper,
        Proceedings,
        Unknown,
        StreamingAudio,
    }

    public class CounterIdentifier
    {
        public IdentifierType IdentifierType { get; set; }

        public string IdentifierValue { get; set; }
    }

    public enum IdentifierType
    {
        OnlineIssn,
        PrintIssn,
        Isbn,
        NoIdentifier,
        OnlineIsbn,
        PrintIsbn,
        Doi,
        Proprietary,
        Database
    }

    public class CounterMetric
    {
        public MetricType MetricType { get; set; }

        public Int32 MetricValue { get; set; }
    }

    public enum MetricType
    {
        /// <summary>
        /// Vendor specific (non counter) metric.
        /// </summary>
        ProprietaryFullTextRequests,

        /// <summary>
        /// Postscript file full text requests.
        /// </summary>
        FullTextPsRequests,

        /// <summary>
        /// Postscript file full text requests formatted for mobile device.
        /// </summary>
        FullTextPsMobileRequests,

        /// <summary>
        /// PDF file full text requests.
        /// </summary>
        FullTextPdfRequests,

        /// <summary>
        /// PDF file full text requests formatted for mobile device.
        /// </summary>
        FullTextPdfMobileRequests,

        /// <summary>
        /// HTML full text requests.
        /// </summary>
        FullTextHtmlRequests,

        /// <summary>
        /// HTML full text requests formatted for mobile device.
        /// </summary>
        FullTextHtmlMobileRequests,

        /// <summary>
        /// Full text requests delivered in EPUB format.
        /// </summary>
        FullTextEpubRequests,

        /// <summary>
        /// Total full text requests.
        /// </summary>
        FullTextTotalRequests,

        /// <summary>
        /// Table of Content Requests.
        /// </summary>
        TocRequests,

        /// <summary>
        /// Request for abstract view of article (detailed article metadata without full text).
        /// </summary>
        AbstractRequests,

        /// <summary>
        /// Number of views to bibliographic references pages associated with an article.
        /// </summary>
        ReferenceRequests,

        /// <summary>
        /// Requests for supplementary data sets referenced in an article.
        /// </summary>
        DataSetRequests,

        /// <summary>
        /// Requests for audio clips referenced in an article.
        /// </summary>
        AudioRequests,

        /// <summary>
        /// Requests for video clips referenced in an article.
        /// </summary>
        VideoRequests,

        /// <summary>
        /// Requests for images referenced in an article.
        /// </summary>
        ImageRequests,

        /// <summary>
        /// Request for podcasts referenced in an article.
        /// </summary>
        PodcastRequests,

        /// <summary>
        /// Number of successful full content requests for multimedia items (includes audio, video, image, podcast and other non-textual content items.
        /// </summary>
        MultimediaRequests,

        /// <summary>
        /// Regular searches as conducted by users (excludes searches attributed to automated search processes and federated searches).
        /// </summary>
        RegularSearches,

        /// <summary>
        /// Searches conducted by federated search or automated search processes.
        /// </summary>
        FederatedSearches,

        /// <summary>
        /// Count of detailed record-views for a database.
        /// </summary>
        RecordViews,

        /// <summary>
        /// Count of clicks originating from a database search result list.
        /// </summary>
        ResultClicks,
        
        /// <summary>
        /// Count of turnaways because simultaneous user limit exceeded.
        /// </summary>
        TurnawayDenials,

        /// <summary>
        /// Number of times users denied access because they were not licensed to access content.
        /// </summary>
        NoLicenseDenials,

        /// <summary>
        /// I don't know yet.
        /// </summary>
        SectionedHtml,
    }
}
