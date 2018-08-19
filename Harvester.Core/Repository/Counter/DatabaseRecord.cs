using System;

namespace ZondervanLibrary.Harvester.Core.Repository.Counter
{
    public class DatabaseRecord
    {
        public String VendorName { get; set; }

        public String DatabaseName { get; set; }

        public Int32? SearchCount { get; set; }

        public Int32? ResultClickCount { get; set; }

        public Int32? RecordViewCount { get; set; }

        public DateTime RunDate { get; set; }
    }
}
