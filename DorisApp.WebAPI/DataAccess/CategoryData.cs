using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.Helpers;

namespace DorisApp.WebAPI.DataAccess
{
    public class CategoryData : BaseDataProcessor
    {
        public override string TableName => "Category";

        public CategoryData(ISqlDataAccess sql, ILogger logger) 
            : base(sql, logger)
        {
        }


        public async Task AddAsync(CategoryModel category, int userId)
        {
            try
            {
                category.CategoryName = category.CategoryName.ToLower();
                category.CreatedByUserId = userId;
                category.UpdatedByUserId = category.CreatedByUserId;
                category.CreatedAt = DateTime.UtcNow;
                category.UpdatedAt = category.CreatedAt;

                await _sql.SaveDataAsync("dbo.spCategoryInsert", category);
                _logger.LogInformation($"Success: Insert {category.CategoryName} Category by {userId} at {category.CreatedAt}");
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"Error: Insert {category.CategoryName} Category by {userId} at {category.CreatedAt} {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<RequestModel<CategoryModel>?> GetByPageAsync(RequestPageDTO request)
        {
            try
            {
                var count = await CountAsync();
                var countPages = AppHelpers.CountPages(count, request.ItemPerPage);

                if (ValidateRequestPageDTO(request, countPages))
                {
                    var output = await _sql.LoadDataAsync<CategoryModel, RequestPageDTO>("dbo.CategoryGetByPage", request);
                    _logger.LogInformation($"Success: Get Category count:{output.Count} at {DateTime.UtcNow}");

                    RequestModel<CategoryModel> reqResult = new()
                    {
                        Models = output,
                        IsInPage = request.PageNo,
                        ItemPerPage = request.ItemPerPage,
                        TotalPages = countPages,
                        TotalItems = count
                    };

                    return reqResult;
                }

                ErrorPage(request);
                return null;
            }

            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }

        }




    }
}
