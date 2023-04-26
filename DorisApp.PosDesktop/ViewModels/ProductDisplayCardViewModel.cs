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

        public ProductPosDisplayModel Product => _product;

        public void HasClicked()
        {
            _getSkuFromPressedProduct.Invoke(_product.Sku);
        }

    }
}
