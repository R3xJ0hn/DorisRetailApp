using Caliburn.Micro;
using DorisApp.Data.Library.API;
using System.Threading.Tasks;
using System;
using DorisApp.PosDesktop.EventModels;
using DorisApp.Data.Library.Model;

namespace DorisApp.PosDesktop.ViewModels
{
    public class LoginViewModel : Screen
    {
        private string _username = "admin@gmail.com";
        private string _password = "1234";
        private string? _errorMessage = string.Empty;
        private readonly IAPIHelper _apiHelper;
        private readonly IEventAggregator _events;
        private AuthenticatedUserModel? _user;

        public LoginViewModel(IAPIHelper apiHelper, IEventAggregator events, AuthenticatedUserModel user)
        {
            _apiHelper = apiHelper;
            _events = events;
            _user = user;
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

        public string? ErrorMessage
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
                _user = await _apiHelper.LogInUserAsync(UserName, Password);

                if (!string.IsNullOrEmpty(_user?.Access_Token))
                {
                    await _events.PublishOnUIThreadAsync(new LogOnEvent(_user));
                }
                else
                {
                    ErrorMessage = _user?.Status;
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
