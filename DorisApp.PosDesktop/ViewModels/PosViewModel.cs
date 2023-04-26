using Caliburn.Micro;
using DorisApp.Data.Library.API;
using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.PosDesktop.Helpers;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks; 
using System.Windows;

namespace DorisApp.PosDesktop.ViewModels
{
    public class PosViewModel : Screen
    {
        private ObservableCollection<ProductDisplayCardViewModel> _catalogCardVM = new();
        private ObservableCollection<SubCategoryItemViewModel> _subCategoriesVM = new();
        private ObservableCollection<CartItemViewModel> _cartItemVM = new();
        private List<CategorySummaryDTO>? _categories = new();
        private readonly TransactionHelper _transaction;
        private readonly SalesEndpoint _salesEndPoint;
        private readonly CategoryEndpoint _categoryEndpoint;
        private readonly SubCategoryEndpoint _subCategoryEndpoint;
        private readonly IConfiguration _config;
        private bool _isBusy;
        private string _timeToday;
        private string _dateToday;
        public string _transctionNumber;
        public string _subTotal { get; set; }
        public string _vat { get; set; }
        public string _discount { get; set; }
        public string _total;

        public PosViewModel(
            TransactionHelper transaction,
            SalesEndpoint salesEndPoint,
            CategoryEndpoint categoryEndpoint,
            SubCategoryEndpoint subCategoryEndpoint,
            IConfiguration config)
        {
            _transaction = transaction;
            _salesEndPoint = salesEndPoint;
            _categoryEndpoint = categoryEndpoint;
            _subCategoryEndpoint = subCategoryEndpoint;
            _config = config;
            _categories?.Add(new CategorySummaryDTO { CategoryName = "All", Id = -1 });
            _timeToday = "";
            _dateToday = "";
            _transctionNumber = transaction.TransctionNumber;
            var timer = new Timer(OnAppTimerChange, null, 1000, 1000);

        }

        public string TransactionNumber => _transctionNumber;

        public List<CategorySummaryDTO>? Categories
        {
            get => _categories;
            private set
            {
                _categories = value;
                NotifyOfPropertyChange(() => Categories);
            }
        }

        public ObservableCollection<SubCategoryItemViewModel> SubCategories
        {
            get => _subCategoriesVM;
            private set
            {
                _subCategoriesVM = value;
                NotifyOfPropertyChange(() => SubCategories);
            }
        }

        public ObservableCollection<ProductDisplayCardViewModel> CatalogItems
        {
            get => _catalogCardVM;
            private set
            {
                _catalogCardVM = value;
                NotifyOfPropertyChange(() => CatalogItems);
            }
        }

        public ObservableCollection<CartItemViewModel> CartItems
        {
            get => _cartItemVM;
            private set
            {
                _cartItemVM = value;
                NotifyOfPropertyChange(() => CartItems);
            }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public string TimeToday
        {
            get => _timeToday;
            set
            {
                _timeToday = value;
                NotifyOfPropertyChange(() => TimeToday);
            }
        }

        public string DateToday
        {
            get => _dateToday;
            set
            {
                _dateToday = value;
                NotifyOfPropertyChange(() => DateToday);
            }
        }

        public string SubTotal
        {
            get => _subTotal;
            set
            {
                _subTotal = value;
                NotifyOfPropertyChange(() => SubTotal);
            }
        }

        public string VAT
        {
            get => _vat;
            set
            {
                _vat = value;
                NotifyOfPropertyChange(() => VAT);
            }
        }

        public string Discount
        {
            get => _discount;
            set
            {
                _discount = value;
                NotifyOfPropertyChange(() => Discount);
            }
        }

        public string Total
        {
            get => _total;
            set
            {
                _total = value;
                NotifyOfPropertyChange(() => Total);
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            try
            {
                var req = new RequestProductPageDTO
                {
                    PageNo = 1,
                    ItemPerPage = 1000,
                    OrderBy = 0
                };

                IsBusy = true;
                await Task.Delay(1000);

                await LoadProductsAsync(req);
                await LoadCategories();
            }
            catch
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";
                await TryCloseAsync();
            }

        }

        private async Task LoadCategories()
        {
            var req = new RequestProductPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1000,
                OrderBy = 0
            };

            _categories?.AddRange((await _categoryEndpoint.GetCategorySummaryAsync(req))?.Data.Models!);
        }

        private async Task LoadSubCategory(int categoryId)
        {
            SubCategories.Clear();
            var _subCategories = await _subCategoryEndpoint.GetSubCategorySummaryByCategoryId(categoryId);

            if (_subCategories!.Data.Count > 0)
            {
                var subCategoryAll = new SubCategoryItemViewModel(OnSubCategoryCLicked,
                    new SubCategorySummaryDTO { SubCategoryName = "All", Id = -1, CategoryId = categoryId });
                subCategoryAll.IsActiveSubCategory = true;
                SubCategories.Add(subCategoryAll);

            }

            foreach (var item in _subCategories!.Data)
            {
                var subCategory = new SubCategoryItemViewModel(OnSubCategoryCLicked, item);

                SubCategories.Add(subCategory);
            }
        }

        private async Task LoadProductsAsync(RequestProductPageDTO request)
        {
            var products = await _salesEndPoint.GetProductAvailableAsync(request);
            var url = _config["URL:apiUrl"] + "/api/file/get/product/";

            _catalogCardVM.Clear();

            if (products!.Data != null)
            {
                var groupedProducts = products.Data.Models
                    .OrderBy(p => p.Sku)
                    .Aggregate(new List<ProductPosDisplayModel>(), (result, p) =>
                    {
                        if (result.Count > 0 && result.Last().Sku == p.Sku)
                        {
                            result.Last().StockAvailable += p.StockAvailable;
                        }
                        else
                        {
                            result.Add(new ProductPosDisplayModel
                            {
                                Id = p.Id,
                                ProductId = p.ProductId,
                                StoredImageName = url + p.StoredImageName,
                                ProductName = p.ProductName,
                                Sku = p.Sku,
                                Size = p.Size,
                                Color = p.Color,
                                StockAvailable = p.StockAvailable,
                                RetailPrice = p.RetailPrice
                            });

                        }
                        return result;
                    });

                foreach (var item in groupedProducts)
                {
                    _catalogCardVM.Add(new ProductDisplayCardViewModel(OnProductClicked, item));
                }
            }

            IsBusy = false;
        }

        private void OnAppTimerChange(object? state)
        {
            lock (this)
            {
                DateToday = DateTime.Now.ToString("dddd, MMM dd yyyy");
                TimeToday = DateTime.Now.ToString("hh:mm ss tt ");
            }
        }

        public async Task OnCategoryChange(CategorySummaryDTO category)
        {
            await LoadSubCategory(category.Id);

            var req = new RequestProductPageDTO
            {
                PageNo = 1,
                ItemPerPage = 100,
                OrderBy = 0,
                CategoryId = category.Id,
            };

            IsBusy = true;

            await LoadProductsAsync(req);
        }

        public async Task OnSubCategoryCLicked(int categoryId, int subCategoryId)
        {
            foreach (var item in SubCategories)
            {
                if (item.Id != subCategoryId)
                    item.IsActiveSubCategory = false;
            }

            var req = new RequestProductPageDTO
            {
                PageNo = 1,
                ItemPerPage = 100,
                OrderBy = 0,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId
            };

            IsBusy = true;
            await LoadProductsAsync(req);
        }

        public void OnProductClicked(string Sku)
        {
            var product = _catalogCardVM.First(x => x.Product.Sku == Sku).Product;
            AddToCart(ref product, 1);
        }

        private void AddToCart(ref ProductPosDisplayModel product, int qty)
        {
            _transaction.AddToCart(ref product, qty);
            UpdateInvoice();
        }

        private void RemoveToCart(ref ProductPosDisplayModel product, int qty)
        {
            _transaction.RemoveToCart(ref product, qty);
            UpdateInvoice();
        }

        private void RemoveItem(CartItemModel model)
        {
            var cartItemView = CartItems.FirstOrDefault(x => x.CartItem.ProductModel!.Id == model.ProductModel!.Id);
            if (cartItemView != null)
            CartItems.Remove(cartItemView);

            var cartItem = _transaction.GetCart().FirstOrDefault(x => x.ProductModel!.Id == model.ProductModel!.Id);
            if (cartItem != null)
                _transaction.GetCart().Remove(cartItem);

            UpdateInvoice();
        }

        private void UpdateQuantities(CartItemModel item)
        {
            var cartItemView = CartItems.FirstOrDefault(x => x.CartItem.ProductModel!.Id == item.ProductModel!.Id);
            if (cartItemView != null)
                cartItemView.Quantity = item.Quantity.ToString();
        }

        private void UpdateInvoice()
        {
            var cart = _transaction.GetCart();

            foreach (var item in cart)
            {
                var cartItemView = CartItems.FirstOrDefault(x => x.CartItem.ProductModel!.Id == item.ProductModel!.Id);
                
                if (cartItemView == null)
                {
                    CartItems.Add(new CartItemViewModel(item, UpdateQuantities, RemoveItem));
                }
                else
                {
                    UpdateQuantities(item);
                }
            }

            SubTotal = _transaction.CalculateSubTotal().ToString("C2");
            VAT = _transaction.CalculateTax().ToString("C2");
            Discount = "0";
            Total = _transaction.CalculateGrandTotal().ToString("C2");
        }


    }


}
