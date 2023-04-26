namespace DorisApp.Data.Library.DTO
{
    public class RequestProductPageDTO : RequestPageDTO
    {
        public string? Sku { get; set; } = null;
        public int CategoryId { get; set; } = -1;
        public int SubCategoryId { get; set; } = -1;

    }
}
