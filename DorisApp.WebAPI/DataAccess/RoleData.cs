using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.Helpers;

namespace DorisApp.WebAPI.DataAccess
{
    public class RoleData : BaseDataProcessor
    {
        public override string TableName => "Roles";

        public RoleData(ISqlDataAccess sql, ILogger logger)
            : base(sql, logger)
        {
        }
        public async Task AddAsync(RoleModel role, int userId)
        {
            role.RoleName = role.RoleName.ToLower();
            role.CreatedByUserId = userId;
            role.UpdatedByUserId = role.CreatedByUserId;
            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedAt = role.CreatedAt;

            try
            {
                await _sql.SaveDataAsync("dbo.spRoleInsert", role);
                _logger.LogInformation($"Success: Insert {role.RoleName} Category by {userId} at {role.CreatedAt}");
            }

            catch (Exception ex)
            {
                ThrowError($"Error: Insert {role.RoleName} Category by {userId} at {role.CreatedAt} {ex.Message}" +
                    Environment.NewLine + ex.Message);
                throw;
            }
        }


        public async Task<RequestModel<RoleModel>?> GetSummaryDataByPageAsync(RequestPageDTO request, int userId)
        {
            return await GetByPageAsync<RoleModel>("dbo.spRoleGetByPage", request, userId);
        }

        public async Task<RoleModel?> GetByIdAsync(int id)
        {
            var p = new { Id = id };
            var output = await _sql.LoadDataAsync<RoleModel, dynamic>("dbo.spRoleGetById", p);
            _logger.LogInformation($"Success: Get Role with RoleId count:{output.Count} at {DateTime.UtcNow}");
            return output.FirstOrDefault();
        }

        public int GetNewUserId()
        {
            return 1;
        }


    }
}
