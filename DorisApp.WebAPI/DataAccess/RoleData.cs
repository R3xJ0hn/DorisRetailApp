using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using System.Data;

namespace DorisApp.WebAPI.DataAccess
{
    public class RoleData : IDisposable, IRoleData
    {
        private readonly ISqlDataAccess _sql;
        private readonly ILogger _logger;

        public RoleData(ISqlDataAccess sql, ILogger logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public void AddRole(RoleModel role, int userId)
        {
            try
            {
                role.RoleName = role.RoleName.ToLower();
                role.CreatedByUserId = userId;
                role.UpdatedByUserId = role.CreatedByUserId;
                role.CreatedAt = DateTime.UtcNow;
                role.UpdatedAt = role.CreatedAt;

                _sql.SaveDataAsync("dbo.spRoleInsert", role);
                _logger.LogInformation($"Success: Insert {role} Category by {userId} at {role.CreatedAt}");
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"Success: Insert {role} Category by {userId} at {role.CreatedAt} {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<RoleModel?> GetRoleByIdAsync(int id)
        {
            var p = new { Id = id };
            var output = await _sql.LoadDataAsync<RoleModel, dynamic>("dbo.spRoleGetById", p);
            _logger.LogInformation($"Success: Get Role with RoleId count:{output.Count} at {DateTime.UtcNow}");
            return output.FirstOrDefault();
        }

        public async Task<List<RoleModel>> GetRolesAsync()
        {
            var output = await _sql.LoadDataAsync<RoleModel>("dbo.spRoleGetAll");
            _logger.LogInformation($"Success: Get Role with RoleId count:{output.Count} at {DateTime.UtcNow}");

            return output;
        }

        public int GetNewUserId()
        {
            return 1;
        }

        public void Dispose()
        {
            _sql.Dispose();
        }
    }
}
