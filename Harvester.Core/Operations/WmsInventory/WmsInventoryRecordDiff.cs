using System;
using ZondervanLibrary.SharedLibrary.Parsing.Conversion;
using ZondervanLibrary.SharedLibrary.Parsing.Fields;
using ZondervanLibrary.SharedLibrary.Parsing.Records;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsInventory
{
    [DelimitedRecord("|", IgnoreFirstLine = true)]
    public class WmsInventoryRecordDiff : IWmsInventoryRecord
    {
        [DelimitedField(IsRequired = true)]
        public InstitutionSymbol InstitutionSymbol { get; set; }

        [DelimitedField(IsRequired = true)]
        public HoldingLocation HoldingLocation { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        public ShelvingLocation? ShelvingLocation { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        public string TemporaryShelvingLocation { get; set; }

        [DelimitedField(IsRequired = true)]
        public ItemType ItemType { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        public string CallNumber { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        public string Author { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Title { get; set; }

        [DelimitedField(IsRequired = false)]
        public string TextualHoldings { get; set; }

        [DelimitedField(IsRequired = true)]
        public MaterialFormat MaterialFormat { get; set; }

        [DelimitedField(IsRequired = false, ValidationPattern = "^[0-9]{1,}$")]
        public int? OclcNumber { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^NULL$")]
        public string Isbn { get; set; }

        [DelimitedField(IsRequired = false/*, ValidationPattern = "^[0-9]{14}$"*/)]
        public string Barcode { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Cost { get; set; }

        [DelimitedField(IsRequired = false)]
        public string StaffNote { get; set; }

        [DelimitedField(IsRequired = false)]
        public string PublicNote { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        public CurrentStatus? CurrentStatus { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss")]
        public DateTime? LoanDateDue { get; set; }

        [DelimitedField(IsRequired = true)]
        public int IssuedCount { get; set; }

        [DelimitedField(IsRequired = true)]
        public int IssuedCountYtd { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss")]
        public DateTime? LastIssuedDate { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss")]
        public DateTime? LastInventoriedDate { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "^-{3}$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss")]
        public DateTime? ItemDeletedDate { get; set; }

        public string Description { get; set; }
    }
}