namespace DorisApp.Data.Library.DTO
{
    public class ProductSummaryDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public int TotalStock { get; set; }
        public int InventoryCount { get; set; }
        public string? StoredImageName { get; set; }
        public bool IsAvailable { get; set; }
    }
}