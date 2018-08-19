using System;
using ZondervanLibrary.SharedLibrary.Parsing.Fields;
using ZondervanLibrary.SharedLibrary.Parsing.Records;

namespace ZondervanLibrary.Harvester.Core.Operations.EZProxy
{
    [DelimitedRecord("\t", IgnoreFirstLine = true)]
    public class EZProxyAudit
    {
        [DelimitedField(IsRequired = true)]
        public DateTime DateTime { get; set; }

        [DelimitedField(IsRequired = true)]
        public string Event { get; set; }

        [DelimitedField(IsRequired = false)]
        public string IP { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Username { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Session { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Other { get; set; }

        public int LineNumber { get; set; }
    }

    public class EZProxyLog
    {
        public string IP { get; set; }

        public string Username { get; set; }

        public DateTime DateTime { get; set; }

        public string Request { get; set; }

        public int HTTPCode { get; set; }

        public int BytesTransferred { get; set; }

        public string Referer { get; set; }

        public string UserAgent { get; set; }

    }
}
