using System;
using ZondervanLibrary.SharedLibrary.Parsing.Conversion;
using ZondervanLibrary.SharedLibrary.Parsing.Fields;
using ZondervanLibrary.SharedLibrary.Parsing.Records;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsInventory
{
    [DelimitedRecord("|", IgnoreFirstLine = true)]
    class WmsInventoryRecord2018 : IWmsInventoryRecord
    {
        [DelimitedField(IsRequired = true)]
        public InstitutionSymbol InstitutionSymbol { get; set; }

        [DelimitedField(IsRequired = true)]
        public HoldingLocation HoldingLocation { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^N\/A$")]
        public ShelvingLocation? ShelvingLocation { get; set; }

        [DelimitedField(IsRequired = false)]
        public string TemporaryShelvingLocation { get; set; }

        [DelimitedField(IsRequired = true)]
        public ItemType ItemType { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^N\/A$")]
        public string CallNumber { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Enumeration_Chronology { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Author { get; set; }

        [DelimitedField(IsRequired = true)]
        public string Title { get; set; }

        [DelimitedField(IsRequired = false)]
        public string ItemMaterialsSpecified { get; set; }

        [DelimitedField(IsRequired = true)]
        public MaterialFormat MaterialFormat { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = "0", ValidationPattern = "^[0-9]{1,}$")]
        public int? OclcNumber { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^N\/A$")]
        public string Isbn { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^(N\/A)|(u{4})$")]
        public string PublicationDate { get; set; }

        [DelimitedField(IsRequired = false /*, ValidationPattern = "^[0-9]{14}$"*/, RemoveCharacters = new[] { "*", " ", "-" })]
        public string Barcode { get; set; }

        [DelimitedField(IsRequired = false)]
        public string Cost { get; set; }

        [DelimitedField(IsRequired = false)]
        public string ItemNonPublicNote { get; set; }

        [DelimitedField(IsRequired = false)]
        public string PublicNote { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^N\/A$")]
        public CurrentStatus? CurrentStatus { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^(0001[-]01[-]01( 00[:]00[:]00)?)|(N\/A)$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd")]
        public DateTime? LoanDateDue { get; set; }

        [DelimitedField(IsRequired = true)]
        public int IssuedCount { get; set; }

        [DelimitedField(IsRequired = true)]
        public int IssuedCountYtd { get; set; }

        [DelimitedField(IsRequired = true)]
        public int SoftIssuedCount { get; set; }

        [DelimitedField(IsRequired = true)]
        public int SoftIssuedCountYtd { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^(0001[-]01[-]01( 00[:]00[:]00)?)|(N\/A)$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd")]
        public DateTime? LastIssuedDate { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^(0001[-]01[-]01( 00[:]00[:]00)?)|(N\/A)$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd")]
        public DateTime? LastInventoriedDate { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^(0001[-]01[-]01( 00[:]00[:]00)?)|(N\/A)$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd")]
        public DateTime? ItemDeletedDate { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^(0001[-]01[-]01)|(N\/A)$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd")]
        public DateTime? DateEnteredOnFile { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^(0001[-]01[-]01( 00[:]00[:]00)?)|(N\/A)$")]
        [DateTimeConversion("yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd")]
        public DateTime? AquiredDate { get; set; }

        [DelimitedField(IsRequired = false, NullPattern = @"^N\/A$")]
        public string LanguageCode { get; set; }

        public string Description { get; set; }

        public string StaffNote { get; set; }
    }
}
