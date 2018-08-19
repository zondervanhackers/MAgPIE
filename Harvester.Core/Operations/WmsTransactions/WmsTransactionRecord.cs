using System;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsTransactions
{
    public class WmsTransactionRecord
    {
        public string ItemBarcode { get; set; }

        public string UserBarcode { get; set; }

        public DateTime LoanDueDate { get; set; }

        public DateTime LoanCheckedOutDate { get; set; }

        public string EventType { get; set; }

        public string InstitutionName { get; set; }

        public DateTime RecordDate { get; set; }
    }
}
