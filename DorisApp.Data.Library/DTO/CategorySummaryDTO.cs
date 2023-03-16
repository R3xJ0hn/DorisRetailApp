namespace DorisApp.Data.Library.DTO
{
    public class CategorySummaryDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; }
        public int SubcategoryCount { get; set; }
        public int ProductCount { get; set; }
    }
}
