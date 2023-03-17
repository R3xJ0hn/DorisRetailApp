namespace DorisApp.Data.Library.DTO
{
    public class SubCategorySummaryDTO
    {
        public int Id { get; set; }
        public string? SubCategoryName { get; set; }
        public string? CategoryName { get; set; }
        public int ProductCount { get; set; }
    }
}
