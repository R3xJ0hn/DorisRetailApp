using Caliburn.Micro;
using DorisApp.Data.Library.API;
using DorisApp.Data.Library.DTO;
using Microsoft.Extensions.Configuration;
using System.Collections.ObjectModel;
using System.Dynamic;
using System.Threading.Tasks;
using System.Windows;

namespace DorisApp.PosDesktop.ViewModels
{
    public class PosViewModel : Screen
    {
        private ObservableCollection<ProductDisplayCardViewModel> _catalogCardVM = new();
        private readonly SalesEndPoint _salesEndPoint;
        private readonly IConfiguration _config;
        private bool _isBusy;

        public PosViewModel(SalesEndPoint salesEndPoint, IConfiguration config)
        {
            _salesEndPoint = salesEndPoint;
            _config = config;
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

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                _isBusy = value;
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        protected override async void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            try
            {
                await LoadProductsAsync();
                IsBusy = false;
            }
            catch
            {
                dynamic settings = new ExpandoObject();
                settings.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                settings.ResizeMode = ResizeMode.NoResize;
                settings.Title = "System Error";
                await TryCloseAsync();
            }

            //NotifyOfPropertyChanges();
        }

        private async Task LoadProductsAsync()
        {
            var req = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 100,
                OrderBy = 0
            };

            IsBusy = true;
            var products = await _salesEndPoint.GetProductAvailableAsync(req);
            var url = _config["URL:apiUrl"] + "/api/file/get/product/";


            if (products!.Data != null)
            {
                foreach (var item in products!.Data.Models)
                {
                    item.StoredImageName = url + item.StoredImageName;
                    _catalogCardVM.Add(new ProductDisplayCardViewModel(item));
                }
            }



        }


    }


}
