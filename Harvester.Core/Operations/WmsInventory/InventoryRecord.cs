using System;

namespace ZondervanLibrary.Harvester.Core.Operations.WmsInventory
{
    public class InventoryRecord
    {
        public int? OclcNumber { get; set; }

        public string Title { get; set; }

        public string MaterialFormat { get; set; }

        public string Author { get; set; }

        public string Barcode { get; set; }

        public string Cost { get; set; }

        public DateTime? LastInventoriedDate { get; set; }

        public DateTime? DeletedDate { get; set; }

        public string ItemType { get; set; }

        public string CallNumber { get; set; }

        public string ShelvingLocation { get; set; }

        public string CurrentStatus { get; set; }

        public string Description { get; set; }

        public DateTime RunDate { get; set; }

        public bool Anomalous { get; set; }
    }
}
