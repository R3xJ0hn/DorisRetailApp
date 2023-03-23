﻿namespace DorisApp.Data.Library.DTO
{
    public class ProductSummaryDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Sku { get; set; }
        public int Stock { get; set; }
        public int InventoryCount { get; set; }
        public string? StoredImageName { get; set; }
        public bool IsAvailable { get; set; }
    }

}


