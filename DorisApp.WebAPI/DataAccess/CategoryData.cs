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
                ThrowError($"Error: Insert {category.CategoryName} Category by {userId} at {category.CreatedAt}" + 
                    Environment.NewLine + ex.Message);
                throw;
            }
        }

        public async Task<RequestModel<CategorySummaryDTO>?> GetSummaryDataByPageAsync(RequestPageDTO request, int userId)
        {
            return await GetByPageAsync<CategorySummaryDTO>("dbo.spCategoryGetSummaryByPage", request, userId);
        }

        public async Task UpdateCategoryAsync(CategoryModel model, int userId)
        {
            model.UpdatedByUserId = userId;
            model.UpdatedAt = DateTime.UtcNow;

            //This will ignore by the stored procedure.
            //This is required to pass the required param.
            model.CreatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spCategoryUpdate", model);
                _logger.LogInformation($"Success: Update Category {model.CategoryName} by {userId} at {model.UpdatedAt}");
            }
            catch (Exception ex)
            {
                ThrowError($"Error: Update Category {model.CategoryName} by {userId} at {model.UpdatedAt}" +
                    Environment.NewLine + ex.Message);
            }
        }

        public async Task DeleteCategoryAsync(CategoryModel model, int userId)
        {
            model.UpdatedByUserId = userId;

            //This will ignore by the stored procedure.
            //This is required to pass the required param.
            model.CreatedAt = DateTime.UtcNow;
            model.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spCategoryDelete", model);
                _logger.LogInformation($"Success: Delete Category {model.CategoryName} by {userId} at {model.UpdatedAt}");
            }
            catch (Exception ex)
            {
                ThrowError($"Error: Delete Category {model.CategoryName} by {userId} at {model.UpdatedAt}" +
                    Environment.NewLine + ex.Message);
            }

        }




    }

}
