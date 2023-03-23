using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{
    public class BrandData : BaseDataProcessor
    {
        public override string TableName => "Brands";

        public BrandData(ISqlDataAccess sql, ILoggerManager logger) : base(sql, logger)
        {
        }

        public async Task<RequestModel<BrandSummaryDTO>?> GetSummaryDataByPageAsync(ClaimsIdentity identity, RequestPageDTO request)
        {
            return await GetByPageAsync<BrandSummaryDTO>(identity, "dbo.spBrandGetSummaryByPage", request);
        }

        public async Task AddAsync(ClaimsIdentity identity, BrandModel brand)
        {
            ValidateFields(identity, brand);

            var userId = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault() ?? "0");
            brand.BrandName = AppHelper.CapitalizeFirstWords(brand.BrandName);
            brand.CreatedByUserId = userId;
            brand.UpdatedByUserId = brand.CreatedByUserId;
            brand.CreatedAt = DateTime.UtcNow;
            brand.UpdatedAt = brand.CreatedAt;

            try
            {
                await _sql.SaveDataAsync("dbo.spBrandInsert", brand);
                _logger.SuccessInsert(identity, brand.BrandName, TableName);
            }

            catch (Exception ex)
            {
                _logger.FailInsert(identity, brand.BrandName, TableName, ex.Message);
                throw;
            }
        }

        public async Task UpdateCategoryAsync(ClaimsIdentity identity, BrandModel brand)
        {
            ValidateFields(identity, brand);
            var exist = await IsExist(brand.Id);

            if (!exist)
            {
                var msg = $"Brand[{brand.BrandName}[{brand.Id}]] not found.";
                _logger.LogError(msg);
                throw new Exception(msg);
            }

            brand.UpdatedByUserId = int.Parse(identity.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "-1");
            brand.UpdatedAt = DateTime.UtcNow;

            //This will ignore by the stored procedure.
            brand.CreatedAt = DateTime.UtcNow;

            //Get the old name
            var oldName = (await GetByIdAsync(identity, brand.Id)).BrandName;

            try
            {
                await _sql.UpdateDataAsync("dbo.spBrandUpdate", brand);
                _logger.SuccessUpdate(identity, brand.BrandName, TableName, oldName);
            }
            catch (Exception ex)
            {
                _logger.FailUpdate(identity, brand.BrandName, TableName, oldName, ex.Message);
                throw;
            }
        }

        public async Task DeleteCategoryAsync(ClaimsIdentity identity, BrandModel brand)
        {
            brand.UpdatedByUserId = int.Parse(identity.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "-1");

            //This will ignore by the stored procedure.
            brand.CreatedAt = DateTime.UtcNow;
            brand.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spBrandDelete", brand);
                _logger.SuccessDelete(identity, brand.BrandName, TableName);
            }
            catch (Exception ex)
            {
                _logger.FailDelete(identity, brand.BrandName, TableName, ex.Message);
                throw;
            }
        }

        public async Task<BrandModel?> GetByIdAsync(ClaimsIdentity identity, int id)
        {
            return await GetByIdAsync<BrandModel>(identity, "dbo.spBrandGetById", id);
        }

        public async Task<bool> IsExist(int id)
        {
            return await IsItemExistAsync<BrandModel>("dbo.spBrandGetById", id);
        }

        public void ValidateFields(ClaimsIdentity identity, BrandModel brand)
        {
            string Name = AppHelper.GetFirstWord(
                identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");
       
            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the brand.";
            }

            if (string.IsNullOrEmpty(brand.BrandName))
            {
                msg = $"The Brand name is null.";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                _logger.LogError($"{Name}: {msg}");
                throw new NullReferenceException(msg);
            }
        }

    }
}
