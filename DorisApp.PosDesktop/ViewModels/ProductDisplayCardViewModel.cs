using Caliburn.Micro;
using DorisApp.Data.Library.Model;
using System;

namespace DorisApp.PosDesktop.ViewModels
{
    public class ProductDisplayCardViewModel : Screen
    {
        private readonly Action<string> _getSkuFromPressedProduct;
        private readonly ProductPosDisplayModel _product;

        public ProductDisplayCardViewModel(Action<string> getSkuFromPressedProduct, ProductPosDisplayModel product)
        {
            _product = product;
            _getSkuFromPressedProduct = getSkuFromPressedProduct;
        }

        public string Id => _product.Id.ToString();
        public string ImagePath => _product.StoredImageName!;
        public string ProductName => _product.ProductName;
        public string Size => _product.Size;
        public string Color => string.IsNullOrEmpty(_product.Color)? "none" : _product.Color;
        public string Stock => _product.StockAvailable.ToString();
        public string Price => _product.RetailPrice.ToString("C");
        public string Sku => _product.Sku;

        public void HasClicked()
        {
            _getSkuFromPressedProduct.Invoke(Sku);
        }

    }
}
