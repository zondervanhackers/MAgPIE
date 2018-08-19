using System;

namespace ZondervanLibrary.SharedLibrary.Sushi
{
    public class SushiRecord
    {
        public string Vendor { get; set; }

        public string Database { get; set; }

        public string Name { get; set; }

        public string PrintISSN { get; set; }

        public string OnlineISSN { get; set; }

        public int FullText { get; set; }

        public DateTime RunDate { get; set; }
    }
}
