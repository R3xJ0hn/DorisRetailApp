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

        public async Task<RequestModel<BrandModel>?> GetSummaryDataByPageAsync(ClaimsIdentity identity, RequestPageDTO request)
        {
            return await GetByPageAsync<BrandModel>(identity, "dbo.spBrandGetByPage", request);
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
                //TODO: Uplaod Image

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
            brand.UpdatedByUserId = int.Parse(identity.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "-1");
            brand.UpdatedAt = DateTime.UtcNow;

            //This will ignore by the stored procedure.
            brand.CreatedAt = DateTime.UtcNow;

            //Get the old name
            var oldName = (await GetByIdAsync(identity, brand.Id)).BrandName;

            ValidateFields(identity, brand, oldName);

            try
            {
                //TODO: Uplaod Image
                //find first and remove if user change it

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



        public async Task<BrandModel> GetByIdAsync(ClaimsIdentity identity, int id)
        {
            return await GetByIdAsync<BrandModel>(identity, "dbo.spBrandGetById", id);
        }

        private void ValidateFields(ClaimsIdentity identity, BrandModel brand, string? oldName = null)
        {
            if (brand.BrandName != null)
            {
                var msg = $"The Category name in Subcategory is null.";
                _logger.FailUpdate(identity, brand.BrandName, TableName, oldName, msg);
                throw new NullReferenceException(msg);
            }
        }

    }
}
