using Caliburn.Micro;
using DorisApp.Data.Library.API;
using System.Threading.Tasks;
using System;

namespace DorisApp.PosDesktop.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _username = "admin@gmail.com";
        private string _password = "Rex123.";
        private string _errorMessage = string.Empty;
        private IAPIHelper _apiHelper;
        private IEventAggregator _events;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events)
        {
            _apiHelper = apiHelper;
            _events = events;
        }

        public string UserName
        {
            get { return _username; }
            set
            {
                _username = value;
                NotifyOfPropertyChange(() => UserName);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                NotifyOfPropertyChange(() => CanLogIn);
            }
        }

        public bool IsErrorVisible
        {
            get
            {
                bool ouput = false;

                if (ErrorMessage?.Length > 0)
                {
                    ouput = true;
                }

                return ouput;
            }
        }

        public string ErrorMessage
        {
            get { return _errorMessage; }
            set
            {
                _errorMessage = value;
                NotifyOfPropertyChange(() => IsErrorVisible);
                NotifyOfPropertyChange(() => ErrorMessage);
            }
        }

        public bool CanLogIn
        {
            get
            {
                bool output = false;

                if (UserName?.Length > 0 && Password?.Length > 0)
                {
                    output = true;
                }

                return output;
            }
        }

        public async Task LogIn()
        {
            try
            {
                ErrorMessage = "";
                var result = await _apiHelper.LogInUserAsync(UserName, Password);
                await _events.PublishOnUIThreadAsync("login");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
