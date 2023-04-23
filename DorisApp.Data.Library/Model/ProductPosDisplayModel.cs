namespace DorisApp.Data.Library.Model
{
    public class ProductPosDisplayModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string StoredImageName { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public string Size { get; set; }
        public string Color { get; set; }
        public int StockAvailable { get; set; }
        public double RetailPrice { get; set; }
    }

}
