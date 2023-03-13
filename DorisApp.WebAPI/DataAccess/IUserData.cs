using DorisApp.Model.Library;

namespace DorisApp.WebAPI.DataAccess
{
    public interface IUserData
    {
        void Dispose();
        Task<UserModel?> FindByEmailAsync(string email);
        Task<UserModel?> FindByIdAsync(string id);
        Task<UserModel> RegisterUserAsync(UserModel user);
        Task SetGenerateRefreshToken(UserModel user);
    }
}