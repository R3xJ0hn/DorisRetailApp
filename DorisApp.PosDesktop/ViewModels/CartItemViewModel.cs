using Caliburn.Micro;
using DorisApp.Data.Library.Model;
using System;

namespace DorisApp.PosDesktop.ViewModels
{
    public class CartItemViewModel:Screen
    {
        private int _quantity;
        private decimal _sum;
        private readonly CartItemModel _cartItemModel;
        private readonly Action<CartItemModel> _cartQuantityChange;
        private readonly Action<CartItemModel> _itemToRemove;

        public CartItemModel CartItem => _cartItemModel;
        public string Sum => _sum.ToString("C");

        public string Quantity
        {
            get => _quantity.ToString();
            set
            {
                int.TryParse(value, out _quantity);
                CartItem.Quantity = _quantity;
                _sum = CartItem.ProductModel!.RetailPrice * _quantity;
                NotifyOfPropertyChange(() => Quantity);
                NotifyOfPropertyChange(() => Sum);
            }
        }

        public CartItemViewModel(CartItemModel cartItemModel, Action<CartItemModel> cartQuantityChange, Action<CartItemModel> itemToRemove)
        {
            _cartItemModel = cartItemModel;
            _cartQuantityChange = cartQuantityChange;
            _itemToRemove = itemToRemove;
            _quantity = _cartItemModel.Quantity;
            _sum = _quantity * _cartItemModel.ProductModel!.RetailPrice;
        }

        public void ReduceQty()
        {
            var qty = (int.Parse(Quantity) - 1);

            if (qty > 0)
            {
                Quantity = qty.ToString();
                _cartItemModel.Quantity = qty;
                _cartQuantityChange.Invoke(_cartItemModel);

            }
        }

        public void AddQty()
        {
            var qty = (int.Parse(Quantity) + 1);

            if (qty < _cartItemModel.ProductModel!.StockAvailable)
            {
                Quantity = qty.ToString();
                _cartItemModel.Quantity = qty;
                _cartQuantityChange.Invoke(_cartItemModel);

            }
        }

        public void CompletyRemoved()
        {
            _itemToRemove.Invoke(_cartItemModel);
        }

    }
}
