﻿
namespace DorisApp.Data.Library.DTO
{
    public class CategoryTableDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int SubcategoryCount { get; set; }
        public int ProductCount { get; set; }
    }
}