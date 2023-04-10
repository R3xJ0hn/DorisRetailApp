using System;

namespace DorisApp.Data.Library.DTO
{
    public class InventorySummaryDTO
    {
        public int Id { get; set; }
        public string Location { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public bool IsProductAvailable { get; set; }
        public string SKU { get; set; } = string.Empty;
        public int StockRemain { get; set; }
        public int StockAvailable { get; set; }
        public int CreatedAt { get; set; }
        public DateTime PurchasedDate { get; set; }
        public DateTime ExpiryDate { get; set; }
        public bool IsAvailable { get; set; }
    }

}