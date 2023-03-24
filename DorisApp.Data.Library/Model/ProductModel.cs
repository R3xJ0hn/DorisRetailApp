using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DorisApp.Data.Library.Model
{
    public class ProductModel
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        public int BrandId { get; set; }

        [Required]
        [RegularExpression(@"^[1-9]\d*(\.\d+)?$",
            ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public int SubcategoryId { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsAvailable { get; set; }

        [Required]
        public string Size { get; set; }
        public string Color { get; set; }

        [Required]
        [DisplayName("Product SKU")]
        public string Sku { get; set; }
        public string? StoredImageName { get; set; }
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
