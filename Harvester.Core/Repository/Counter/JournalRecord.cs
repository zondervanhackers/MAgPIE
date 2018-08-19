using System;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public class JournalRecord
    {
        public string VendorName { get; set; }

        public string DatabaseName { get; set; }

        public string JournalName { get; set; }

        public string PrintIssn { get; set; }

        public string OnlineIssn { get; set; }

        public Int32 FullTextCount { get; set; }

        public DateTime RunDate { get; set; }
    }
}
