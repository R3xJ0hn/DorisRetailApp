using DorisApp.Data.Library.Model;

namespace DorisApp.WebAPI.DataAccess
{
    public interface IRoleData
    {
        void AddRole(RoleModel role, int userId);
        Task<int> CountRolesAsync();
        void Dispose();
        int GetNewUserId();
        Task<RoleModel?> GetRoleByIdAsync(int id);
        Task<List<RoleModel>> GetRoleByPageNumAsync(int page);
    }
}