using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace DorisApp.Data.Library.Model
{
    public class InventoryModel
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [DisplayName("Location")]
        public string Location { get; set; } =string.Empty;

        [Required]
        public double PurchasePrice { get; set; }

        [Required]
        public double RetailPrice { get; set; }

        [Required]
        public int Quantity { get; set; }

        public int StockRemain { get; set; }

        public int StockAvailable { get; set; }

        public bool IsAvailable { get; set; }

        [Required]
        public DateTime PurchasedDate { get; set; }

        [Required]
        public DateTime ExpiryDate { get; set; }

        public int CreatedByUserId { get; set; }
        public int UpdatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
