using DorisApp.Data.Library.Model;

namespace DorisApp.PosDesktop.EventModels
{
    public class LogOnEvent
    {
        private AuthenticatedUserModel _authenticatedUser;
        public AuthenticatedUserModel AuthenticatedUser => _authenticatedUser;

        public LogOnEvent(AuthenticatedUserModel user)
        {
            _authenticatedUser = user;
        }
    }
}
