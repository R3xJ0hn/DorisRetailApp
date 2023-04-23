using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DorisApp.Data.Library.Model
{
    public class ProductModel
    {
        public int Id { get; set; }

        public int BrandId { get; set; }

        [Required]
        [RegularExpression(@"^[1-9]\d*(\.\d+)?$",
            ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public int SubCategoryId { get; set; }
        public bool IsTaxable { get; set; }
        public bool IsAvailable { get; set; }

        public string? StoredImageName { get; set; }
        public string? Description { get; set; }
        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        private string productName = string.Empty;
        private string sku = string.Empty;
        private string size = string.Empty;
        private string color = string.Empty;


        [Required]
        [DisplayName("Product Name")]
        public string ProductName
        {
            get { return productName; }
            set { productName = value.Trim(); }
        }

        [Required]
        [DisplayName("Product SKU")]
        public string Sku
        {
            get { return sku; }
            set { sku = value.Trim(); }
        }

        [Required]
        public string Size
        {
            get { return size; }
            set { size = value.Trim(); }
        }

        public string Color
        {
            get { return color; }
            set { color = value.Trim(); }
        }

    }
}
