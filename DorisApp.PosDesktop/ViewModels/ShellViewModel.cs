using Caliburn.Micro;
using DorisApp.Data.Library.API;
using DorisApp.PosDesktop.EventModels;
using System.Threading.Tasks;
using System.Threading;
using DorisApp.PosDesktop.Views;
using Microsoft.Extensions.Configuration;

namespace DorisApp.PosDesktop.ViewModels
{

    public class ShellViewModel : Conductor<object>, IHandle<LogOnEvent>
    {
        private IEventAggregator _events;
        private PosViewModel _posViewModel;
        private readonly IConfiguration _config;
        private IAPIHelper _apiHelper;

        public ShellViewModel(IEventAggregator events, IAPIHelper apiHelper, PosViewModel posViewModel, IConfiguration config)
        {
            _events = events;
            _apiHelper = apiHelper;
            _posViewModel = posViewModel;
            _config = config;
            ActiveItem = IoC.Get<LoginViewModel>();
            _events.SubscribeOnPublishedThread(this);
            this.DisplayName = _config["StoreInfo:StoreName"];
        }

        public Task HandleAsync(LogOnEvent message, CancellationToken cancellationToken)
        {
            ActiveItem = _posViewModel;
            return Task.CompletedTask;
        }

        public void LogOut()
        {
            _apiHelper.LogOffUser();
            ActiveItem = IoC.Get<LoginViewModel>();
            //_salesViewModel.Exit(); TODO: Exit the Pos
        }
    }
}
