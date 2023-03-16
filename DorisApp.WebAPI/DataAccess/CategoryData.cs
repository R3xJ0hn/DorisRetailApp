using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.Helpers;

namespace DorisApp.WebAPI.DataAccess
{
    public class CategoryData : BaseDataProcessor
    {
        public override string TableName => "Categories";

        public CategoryData(ISqlDataAccess sql, ILogger logger)
            : base(sql, logger)
        {
        }

        public async Task AddAsync(CategoryModel category, int userId)
        {
            if (category == null)
            {
                throw new ArgumentNullException(nameof(category));
            }

            if (userId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(userId));
            }

            category.CategoryName = category.CategoryName.ToLower();
            category.CreatedByUserId = userId;
            category.UpdatedByUserId = category.CreatedByUserId;
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = category.CreatedAt;

            try
            {
                await _sql.SaveDataAsync("dbo.spCategoryInsert", category);
                _logger.LogInformation($"Success: Insert {category.CategoryName} Category by {userId} at {category.CreatedAt}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error: Insert {category.CategoryName} Category by {userId} at {category.CreatedAt}", ex.Message);
                throw;
            }
        }

        public async Task<RequestModel<CategoryTableDTO>?> GetTableDataByPageAsync(RequestPageDTO request)
        {
            return await GetByPageAsync<CategoryTableDTO>("dbo.spCategoryGetByPage", request);
        }

        public async Task UpdateCategoryAsync(CategoryModel model, int userId)
        {
            model.UpdatedByUserId = userId;
            model.UpdatedAt = DateTime.UtcNow;

            //This is ignore by the stored procedure.
            //This is required to pass the required param.
            model.CreatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spCategoryUpdate", model);
                _logger.LogInformation($"Success: Update Category {model.CategoryName} by {userId} at {model.UpdatedAt}");
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error: Update Category {model.CategoryName} by {userId} at {model.UpdatedAt}";
                _logger.LogError(errorMsg, ex.Message);
                throw new ArgumentException(errorMsg);
            }
        }

    }

}
