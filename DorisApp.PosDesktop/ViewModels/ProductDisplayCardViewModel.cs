using Caliburn.Micro;
using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;

namespace DorisApp.PosDesktop.ViewModels
{
    public class ProductDisplayCardViewModel : Screen
    {
        private readonly ProductPosDisplayModel _product;

        public ProductDisplayCardViewModel(ProductPosDisplayModel product)
        {
            _product = product;
        }

        public string Id => _product.Id.ToString();
        public string ImagePath => _product.StoredImageName!;
        public string ProductName => _product.ProductName;
        public string Size => _product.Size;
        public string Color => string.IsNullOrEmpty(_product.Color)? "none" : _product.Color;
        public string Stock => _product.StockAvailable.ToString();
        public string Price => _product.RetailPrice.ToString("C");
    }
}
