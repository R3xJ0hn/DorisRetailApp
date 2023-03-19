using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{
    public class CategoryData : BaseDataProcessor
    {
        public override string TableName => "Categories";

        public CategoryData(ISqlDataAccess sql, ILoggerManager logger) : base(sql, logger)
        {
        }
        public async Task<RequestModel<CategorySummaryDTO>?> GetSummaryDataByPageAsync(ClaimsIdentity identity, RequestPageDTO request)
        {
            return await GetByPageAsync<CategorySummaryDTO>(identity, "dbo.spCategoryGetSummaryByPage", request);
        }

        public async Task AddAsync(ClaimsIdentity identity, CategoryModel category)
        {
            ValidateFields(identity, category);

            var userId = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault() ?? "0");
            category.CategoryName = AppHelper.CapitalizeFirstWords(category.CategoryName);
            category.CreatedByUserId = userId;
            category.UpdatedByUserId = category.CreatedByUserId;
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = category.CreatedAt;

            try
            {
                await _sql.SaveDataAsync("dbo.spCategoryInsert", category);
                _logger.SuccessInsert(identity, category.CategoryName, TableName);
            }
            catch (Exception ex)
            {
                _logger.FailInsert(identity, category.CategoryName, TableName,ex.Message);
                throw;
            }
        }

        public async Task UpdateCategoryAsync(ClaimsIdentity identity, CategoryModel category)
        {
            ValidateFields(identity, category);

            category.UpdatedByUserId = int.Parse(identity.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "-1");
            category.UpdatedAt = DateTime.UtcNow;

            //This will ignore by the stored procedure.
            category.CreatedAt = DateTime.UtcNow;

            //Get the old name
            var oldName = (await GetByIdAsync (identity, category.Id)).CategoryName;

            try
            {
                await _sql.UpdateDataAsync("dbo.spCategoryUpdate", category);
                _logger.SuccessUpdate(identity,category.CategoryName,TableName,oldName);
            }
            catch (Exception ex)
            {
                _logger.FailUpdate(identity,category.CategoryName,TableName,oldName,ex.Message);
                throw;
            }
        }

        public async Task DeleteCategoryAsync(ClaimsIdentity identity, CategoryModel category)
        {
            category.UpdatedByUserId = int.Parse(identity.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "-1");

            //This will ignore by the stored procedure.
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spCategoryDelete", category);
                _logger.SuccessDelete(identity,category.CategoryName,TableName);
            }
            catch (Exception ex)
            {
                _logger.FailDelete(identity,category.CategoryName,TableName, ex.Message);
                throw;
            }
        }

        public async Task<CategoryModel> GetByIdAsync(ClaimsIdentity identity, int id)
        {
            return await GetByIdAsync<CategoryModel>(identity, "dbo.spCategoryGetById", id);
        }

        public async Task<bool> IsExist(int id)
        {
            return await IsItemExistAsync<CategoryModel>("dbo.spCategoryGetById", id);
        }

        private void ValidateFields(ClaimsIdentity identity, CategoryModel category)
        {
            string Name = AppHelper.GetFirstWord(
                identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");

            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the category.";
            }

            if (string.IsNullOrWhiteSpace(category.CategoryName))
            {
                msg = $"The Category name in Subcategory is null.";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                _logger.LogError($"{Name}: {msg}");
                throw new NullReferenceException(msg);
            }
        }

    }
}
