namespace DorisApp.Data.Library.DTO
{
    public class InventorySummaryDTO
    {
        public string Loaction { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int StockRemain { get; set; }
        public int CreatedAt { get; set; }
        public int ExpiryDate { get; set; }
        public bool Availability { get; set; }
    }

}