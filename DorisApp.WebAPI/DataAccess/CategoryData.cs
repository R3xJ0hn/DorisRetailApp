using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;

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
                _logger.LogError(ex, $"Error: Insert {category.CategoryName} Category by {userId} at {category.CreatedAt}");
                throw;
            }
        }

        public async Task<RequestModel<CategoryTableDTO>?> GetTableDataByPageAsync(RequestPageDTO request)
        {
            return await GetByPageAsync<CategoryTableDTO>("dbo.spCategoryGetByPage", request);
        }

    }

}
