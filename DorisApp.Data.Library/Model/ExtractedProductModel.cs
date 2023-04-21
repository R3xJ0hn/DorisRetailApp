namespace DorisApp.Data.Library.Model
{
    public class ExtractedProductModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Sku { get; set; } = string.Empty;
        public string BrandName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string SubCategory { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Size { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
    }
}
