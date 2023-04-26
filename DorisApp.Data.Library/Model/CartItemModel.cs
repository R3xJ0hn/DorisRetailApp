namespace DorisApp.Data.Library.Model
{
    public class CartItemModel
    {
        public int Id { get; set; }
        public int SaleId { get; set; }
        public int InventoryId { get; set; }
        public string InvoiceNum { get; set; } = string.Empty;
        public ProductPosDisplayModel? ProductModel { get; set; }
        public int Quantity { get; set; }
    }

}
