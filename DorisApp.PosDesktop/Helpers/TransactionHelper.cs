using DorisApp.Data.Library.API;
using DorisApp.Data.Library.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;
using System.Linq;
using Microsoft.Extensions.Configuration;

namespace DorisApp.PosDesktop.Helpers
{
    public class TransactionHelper
    {
        private readonly AuthenticatedUserModel _user;
        private readonly List<CartItemModel> _cart = new();
        private readonly SalesEndpoint _saleEndpoint;
        private readonly IConfiguration _config;

        public TransactionHelper(AuthenticatedUserModel user, SalesEndpoint saleEndpoint, IConfiguration config)
        {
            _user = user;
            _saleEndpoint = saleEndpoint;
            _config = config;
            string genId = DateTime.Now.ToString("ddmmyyhhmmss") + _user.Id.ToString("D3");
            char[] reverseGenId = genId.ToCharArray();
            Array.Reverse(reverseGenId);
            TransctionNumber = new string(reverseGenId);
        }

        public List<CartItemModel> GetCart() => _cart;
        public string TransctionNumber { get; }

        public CartItemModel AddToCart(ref ProductPosDisplayModel product, int qty)
        {
            if (product.StockAvailable >= qty)
            {
                var id = product.Id;
                var foundItem = _cart.SingleOrDefault(p => p.ProductModel?.Id == id);
                product.StockAvailable -= qty;

                if (foundItem != null)
                {
                    foundItem.Quantity += qty;
                    return foundItem;
                }
                else
                {
                    CartItemModel cartItem = new()
                    {
                        ProductModel = product,
                        Quantity = qty
                    };

                    _cart.Add(cartItem);
                    return cartItem;
                }
            }
            else
            {
                throw new Exception("Unable to add product. The qty given is " +
                            "greater than the product stock in the inventory.");
            }
        }

        public CartItemModel? RemoveToCart(ref ProductPosDisplayModel product, int qty)
        {
            var id = product.Id;
            var foundItem = _cart.SingleOrDefault(p => p.ProductModel?.Id == id);
            product.StockAvailable -= qty;

            if (foundItem != null)
            {
                foundItem.ProductModel!.StockAvailable += qty;

                if (foundItem.Quantity > 1)
                {
                    foundItem.Quantity -= 1;
                }
                else
                {
                    _cart.Remove(foundItem);
                }
            }

            return foundItem;
        }

        public void CompletelyRemoveProduct(ref ProductPosDisplayModel product)
        {
            var id = product.Id;
            var foundItem = _cart.SingleOrDefault(p => p.ProductModel?.Id == id);
            if (foundItem != null)
            _cart.Remove(foundItem);
        }

        public decimal CalculateSubTotal()
        {
            decimal subtotal = 0;
            subtotal = _cart.Sum(x => x.ProductModel!.RetailPrice * x.Quantity);
            return subtotal;
        }

        public decimal CalculateTax()
        {
            decimal totalTaxAmount = 0;
            decimal.TryParse(_config["AppSettings:tax"], out decimal tax);

            totalTaxAmount = _cart
                .Where(x => x.ProductModel!.IsTaxable)
                .Sum(x => (tax / 100)
                       * x.ProductModel!.RetailPrice * x.Quantity);
            return totalTaxAmount;
        }

        public decimal CalculateGrandTotal()
        {
            return CalculateSubTotal() + CalculateTax();
        }

        public async Task CheckOut()
        {
            SaleModel saleModel = new()
            {
                TransactionNum = TransctionNumber,
                Subtotal = CalculateSubTotal(),
                TotalTax = CalculateTax(),
                Total = CalculateGrandTotal(),
                CashierId = _user.Id.ToString(),
                SaleDate = DateTime.UtcNow,
                ProcessIn = $"{Environment.UserName}:{Environment.MachineName}"
            };


            foreach (var item in _cart)
            {
                saleModel.CartItems.Add(item);
            }

            //TODO await _saleEndpoint.PostSale(saleModel);
            _cart.Clear();
        }

        public void Exit()
        {
            _cart.Clear();
        }

    }
}
