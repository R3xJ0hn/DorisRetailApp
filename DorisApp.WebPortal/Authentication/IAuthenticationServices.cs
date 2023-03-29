using DorisApp.WebPortal.Model;

namespace DorisApp.WebPortal.Authentication
{
    public interface IAuthenticationServices
    {
        Task<AuthenticatedUserModel> Login(AuthenticationUserModel userAuth, bool saveToken);
        Task Logout();
    }
}