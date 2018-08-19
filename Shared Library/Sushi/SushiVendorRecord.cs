using System;

namespace ZondervanLibrary.SharedLibrary.Sushi
{
    public class SushiVendorRecord
    {
        public string Vendor { get; set; }

        public string Database { get; set; }

        public int? Searches { get; set; }

        public int? Result_Clicks { get; set; }

        public int? Record_Views { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
