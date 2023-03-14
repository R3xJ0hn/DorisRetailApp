using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
                _logger.LogInformation($"Error: Insert {role} Category by {userId} at {role.CreatedAt} {ex.Message}");
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

        public async Task<List<RoleModel>> GetRoleByPageNumAsync(int page)
        {
            var getPageCount = await CountRolesAsync();

            if (page > getPageCount || page <= 0)
            {
                string msg = $"Error: Page {page} is out of range. The database have only {getPageCount} pages.";
                _logger.LogInformation(msg);
                throw new ArgumentException(msg);
            }

            var p = new { PageNo = page };
            var output = await _sql.LoadDataAsync<RoleModel, dynamic>("dbo.spRoleGetByPage", p);
            _logger.LogInformation($"Success: Get Role with RoleId count:{output.Count} at {DateTime.UtcNow}");

            return output;
        }

        public async Task<int> CountRolesAsync()
        {
            return await _sql.CountPageAsync("Roles");
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
