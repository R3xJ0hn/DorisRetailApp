using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{
    public class RoleData : BaseDataProcessor
    {
        public override string TableName => "Roles";

        public RoleData(ISqlDataAccess sql, ILoggerManager logger) : base(sql, logger)
        {
        }

        public async Task<RequestModel<RoleModel>?> GetSummaryDataByPageAsync(ClaimsIdentity identity, RequestPageDTO request)
        {
            return (await GetByPageAsync<RoleModel>(identity, "dbo.spRoleGetByPage", request)).Data;
        }

        public async Task AddAsync(ClaimsIdentity identity, RoleModel role)
        {
            ValidateFields(identity, role);
            var userId = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault() ?? "0");
            role.RoleName = AppHelper.CapitalizeFirstWords(role.RoleName);
            role.CreatedByUserId = userId;
            role.UpdatedByUserId = role.CreatedByUserId;
            role.CreatedAt = DateTime.UtcNow;
            role.UpdatedAt = role.CreatedAt;

            try
            {
                await _sql.SaveDataAsync("dbo.spRoleInsert", role);
                _logger.SuccessInsert(identity, role.RoleName, TableName);
            }

            catch (Exception ex)
            {
                _logger.FailInsert(identity, role.RoleName, TableName,ex.Message);
                throw;
            }
        }

        public async Task<RoleModel> GetByIdAsync(ClaimsIdentity identity, int id)
        {
            return await GetByIdAsync<RoleModel>(identity,"dbo.spRoleGetById", id);
        }

        public async Task<bool> IsExist(int id)
        {
            return await IsItemExistAsync<RoleModel>("dbo.spRoleGetById", id);
        }

        public int GetIdForNewUser()
        {
            //TODO: Get the id for Anonymous user.
            return 1;
        }

        private void ValidateFields(ClaimsIdentity identity, RoleModel role)
        {
            string Name = AppHelper.GetFirstWord(
                identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");

            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the role.";
            }

            if (string.IsNullOrEmpty(role.RoleName))
            {
                msg = $"The Role name is null.";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                _logger.LogError($"{Name}: {msg}");
                throw new NullReferenceException(msg);
            }
        }

    }
}
