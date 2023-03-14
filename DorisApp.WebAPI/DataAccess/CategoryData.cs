using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;

namespace DorisApp.WebAPI.DataAccess
{
    public class CategoryData : IDisposable, ICategoryData
    {
        private readonly ISqlDataAccess _sql;
        private readonly ILogger _logger;

        public CategoryData(ISqlDataAccess sql, ILogger logger)
        {
            _sql = sql;
            _logger = logger;
        }
        public void AddNewCategory(CategoryModel category, int userId)
        {
            try
            {
                category.CategoryName = category.CategoryName.ToLower();
                category.CreatedByUserId = userId;
                category.UpdatedByUserId = category.CreatedByUserId;
                category.CreatedAt = DateTime.UtcNow;
                category.UpdatedAt = category.CreatedAt;

                _sql.SaveDataAsync("dbo.spCategoryInsert", category);
                _logger.LogInformation($"Success: Insert {category} Category by {userId} at {category.CreatedAt}");
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"Error: Insert {category} Category by {userId} at {category.CreatedAt} {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
        }

        public void Dispose()
        {
            _sql.Dispose();
        }
    }
}
