using DorisApp.Model.Library;

namespace DorisApp.WebAPI.DataAccess
{
    public interface IRoleData
    {
        void AddRole(RoleModel role, int userId);
        void Dispose();
        int GetNewUserId();
        Task<RoleModel?> GetRoleByIdAsync(int id);
        Task<List<RoleModel>> GetRolesAsync();
    }
}