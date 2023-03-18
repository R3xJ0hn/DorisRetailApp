using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Data;
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
            ValidateFields(identity, subCategory);

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
            ValidateFields(identity, subCategory);

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
                throw new Exception(ex.Message);
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
                throw new Exception(ex.Message);
            }
        }

        public async Task<SubCategoryModel> GetByIdAsync(ClaimsIdentity identity, int id)
        {
            return await GetByIdAsync<SubCategoryModel>(identity, "dbo.spSubCategoryGetById", id);
        }

        private void ValidateFields(ClaimsIdentity identity, SubCategoryModel subCategory)
        {
            string Name = AppHelper.GetFirstWord(
                identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");

            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the sub category.";
            }

            if (subCategory.CategoryId == 0)
            {
                msg = $"The Category ID in {subCategory.SubCategoryName} Subcategory is zero.";
            }

            if (string.IsNullOrWhiteSpace(subCategory.SubCategoryName))
            {
                msg = $"The Sub Category name in Subcategory is null.";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                _logger.LogError($"{Name}: {msg}");
                throw new NullReferenceException(msg);
            }
        }
    }
}
