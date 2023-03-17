﻿using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
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
            return await GetByPageAsync<RoleModel>(identity, "dbo.spRoleGetByPage", request);
        }

        public async Task AddAsync(ClaimsIdentity identity, RoleModel role)
        {
            var userId = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault() ?? "0");
            role.RoleName = role.RoleName.ToLower();
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

        public int GetIdForNewUser()
        {
            //TODO: Get the id for Anonymous user.
            return 1;
        }

    }
}
