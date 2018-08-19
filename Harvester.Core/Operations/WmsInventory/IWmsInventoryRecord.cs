using System;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsInventory
{
    public interface IWmsInventoryRecord
    {
        InstitutionSymbol InstitutionSymbol { get; set; }
        string Barcode { get; set; }
        int? OclcNumber { get; set; }
        string Title { get; set; }
        MaterialFormat MaterialFormat { get; set; }
        string Author { get; set; }
        string CallNumber { get; set; }
        string Description { get; set; }
        ShelvingLocation? ShelvingLocation { get; set; }
        HoldingLocation HoldingLocation { get; set; }
        string Cost { get; set; }
        string TemporaryShelvingLocation { get; set; }
        CurrentStatus? CurrentStatus { get; set; }
        DateTime? LoanDateDue { get; set; }
        DateTime? ItemDeletedDate { get; set; }
        DateTime? LastInventoriedDate { get; set; }
        int IssuedCount { get; set; }
        int IssuedCountYtd { get; set; }
        DateTime? LastIssuedDate { get; set; }
        ItemType ItemType { get; set; }
        string StaffNote { get; set; }
        string PublicNote { get; set; }
        string Isbn { get; set; }
    }
}