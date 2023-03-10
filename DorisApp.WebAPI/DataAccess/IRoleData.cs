using DorisApp.WebAPI.Model;

namespace DorisApp.WebAPI.DataAccess
{
    public interface IRoleData
    {
        void AddRole(RoleModel role, int userId);
        void Dispose();
        Task<RoleModel?> GetRoleByIdAsync(int id);
        Task<List<RoleModel>> GetRolesAsync();
    }
}