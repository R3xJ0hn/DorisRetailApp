using System.Collections.Generic;
using System;

namespace DorisApp.Data.Library.Model
{
    public class SaleModel
    {
        public string TransactionNum { get; set; }
        public string CashierId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalTax { get; set; }
        public decimal Total { get; set; }
        public DateTime SaleDate { get; set; }
        public string ProcessIn { get; set; }
        public int CreatedByUserId { get; set; }
        public DateTime CreatedAt { get; set; }

        public List<CartItemModel> CartItems { get; set; } = new List<CartItemModel>();
    }

}
