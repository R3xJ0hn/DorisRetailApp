using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{
    public class SubCategoryData : BaseDataProcessor
    {
        public override string TableName => "SubCategories";

        public SubCategoryData(ISqlDataAccess sql, ILoggerManager logger) : base(sql, logger)
        {
        }

        public async Task<RequestModel<SubCategorySummaryDTO>?> GetSummaryDataByPageAsync(ClaimsIdentity identity, RequestPageDTO request)
        {
            return await GetByPageAsync<SubCategorySummaryDTO>(identity, "dbo.spSubCategoryGetSummaryByPage", request);
        }

        public async Task AddAsync(ClaimsIdentity identity, SubCategoryModel subCategory)
        {
            var userId = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault() ?? "0");
            subCategory.SubCategoryName = AppHelper.CapitalizeFirstWords(subCategory.SubCategoryName);
            subCategory.CategoryId= subCategory.CategoryId;
            subCategory.CreatedByUserId = userId;
            subCategory.UpdatedByUserId = subCategory.CreatedByUserId;
            subCategory.CreatedAt = DateTime.UtcNow;
            subCategory.UpdatedAt = subCategory.CreatedAt;

            try
            {
                await _sql.SaveDataAsync("dbo.spSubCategoryInsert", subCategory);
                _logger.SuccessInsert(identity, subCategory.SubCategoryName, TableName);
            }
            catch (Exception ex)
            {
                _logger.FailInsert(identity, subCategory.SubCategoryName, TableName, ex.Message);
                throw;
            }
        }

        public async Task UpdateCategoryAsync(ClaimsIdentity identity, SubCategoryModel subCategory)
        {
            subCategory.UpdatedByUserId = int.Parse(identity.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "-1");
            subCategory.UpdatedAt = DateTime.UtcNow;

            //This will ignore by the stored procedure.
            subCategory.CreatedAt = DateTime.UtcNow;

            //Get the old name
            string oldName = (await GetByIdAsync (identity, subCategory.Id)).SubCategoryName;

            try
            {
                await _sql.UpdateDataAsync("dbo.spSubCategoryUpdate", subCategory);
                _logger.SuccessUpdate(identity, subCategory.SubCategoryName, TableName, oldName);
            }
            catch (Exception ex)
            {
                _logger.FailUpdate(identity, subCategory.SubCategoryName, TableName, oldName, ex.Message);
                throw;
            }
        }


        public async Task DeleteCategoryAsync(ClaimsIdentity identity, SubCategoryModel subCategory)
        {
            subCategory.UpdatedByUserId = int.Parse(identity.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "-1");

            //This will ignore by the stored procedure.
            subCategory.CreatedAt = DateTime.UtcNow;
            subCategory.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spSubCategoryDelete", subCategory);
                _logger.SuccessDelete(identity, subCategory.SubCategoryName, TableName);
            }
            catch (Exception ex)
            {
                _logger.FailDelete(identity, subCategory.SubCategoryName, TableName, ex.Message);
                throw;
            }
        }

        public async Task<SubCategoryModel> GetByIdAsync(ClaimsIdentity identity, int id)
        {
            return await GetByIdAsync<SubCategoryModel>(identity, "dbo.spSubCategoryGetById", id);
        }



    }
}
