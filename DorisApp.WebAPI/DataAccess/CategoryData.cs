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
        public async Task<ResultDTO<RequestModel<CategorySummaryDTO>?>> GetSummaryDataByPageAsync(ClaimsIdentity? identity, RequestPageDTO request)
        {
            return await GetByPageAsync<CategorySummaryDTO>(identity, "dbo.spCategoryGetSummaryByPage", request);
        }

        public async Task<ResultDTO<List<CategorySummaryDTO>>> AddCategoryAsync(ClaimsIdentity? identity, CategoryModel category)
        {
            try
            {
                var userId = int.Parse(identity?.Claims.FirstOrDefault(c => 
                c.Type == ClaimTypes.NameIdentifier)?.Value ?? "1");

                category.CategoryName = AppHelper.CapitalizeFirstWords(category.CategoryName);
                category.CreatedByUserId = userId;
                category.UpdatedByUserId = category.CreatedByUserId;
                category.CreatedAt = DateTime.UtcNow;
                category.UpdatedAt = category.CreatedAt;

                var getIdentical = await _sql.LoadDataAsync<CategoryModel,
                    CategoryModel>("spCategoryGetIdentical", category);


                if (getIdentical.Count > 0)
                {
                    var categoryModel = getIdentical.FirstOrDefault();
                    await _sql.UpdateDataAsync("dbo.spCategoryRestore", categoryModel);

                    //If it is just mark as deleted
                    if (categoryModel != null && categoryModel.CategoryName == "*")
                    {
                        categoryModel.CategoryName = category.CategoryName;
                        categoryModel.UpdatedByUserId = userId;
                        categoryModel.UpdatedAt = DateTime.UtcNow;

                        await _sql.UpdateDataAsync("dbo.spCategoryRestore", categoryModel);
                        return new ResultDTO<List<CategorySummaryDTO>>
                        {
                            ErrorCode = 0,
                            IsSuccessStatusCode = true,
                            ReasonPhrase = "Successfully restore category."
                        };
                    }

                    //If it is not deleted
                    if (categoryModel != null && categoryModel.CategoryName != "*")
                    {
                        return new ResultDTO<List<CategorySummaryDTO>>
                        {
                            ErrorCode = 3,
                            IsSuccessStatusCode = false,
                            ReasonPhrase = $"Category not saved: {getIdentical.Count} identical item(s) found."
                        };
                    }

                }

                await ValidateFields(identity, category);
                await _sql.SaveDataAsync("dbo.spCategoryInsert", category);
                await _logger.SuccessInsert(identity, category.CategoryName, TableName);

                return new ResultDTO<List<CategorySummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully added new category."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailInsert(identity, category.CategoryName, TableName, ex.Message);
                return new ResultDTO<List<CategorySummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<List<CategorySummaryDTO>>> UpdateCategoryAsync(ClaimsIdentity? identity, CategoryModel category)
        {
            var oldName = string.Empty;

            try
            {
                var getExistingItem = await GetByIdAsync(identity, category.Id);
                oldName = (await GetByIdAsync(identity, category.Id))?.CategoryName;

                if (getExistingItem == null)
                {
                    var msg = $"Category[{category.CategoryName}[{category.Id}]] not found.";
                    await _logger.LogError(msg);
                    return new ResultDTO<List<CategorySummaryDTO>>
                    {
                        ErrorCode = 5,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = msg
                    };
                }

                category.CategoryName = AppHelper.CapitalizeFirstWords(category.CategoryName);
                category.UpdatedByUserId = int.Parse(identity?.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault() ?? "1");
                category.UpdatedAt = DateTime.UtcNow;

                //This will ignore by the stored procedure.
                category.CreatedAt = DateTime.UtcNow;

                await ValidateFields(identity, category);
                await _sql.UpdateDataAsync("dbo.spCategoryUpdate", category);
                await _logger.SuccessUpdate(identity, category.CategoryName, TableName, oldName ?? "");

                return new ResultDTO<List<CategorySummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully update category."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailUpdate(identity, category.CategoryName, TableName, oldName ?? "", ex.Message);
                return new ResultDTO<List<CategorySummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<CategorySummaryDTO>> DeleteCategoryAsync(ClaimsIdentity? identity, CategoryModel category)
        {
            category.UpdatedByUserId = int.Parse(identity?.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "1");

            //This will ignore by the stored procedure.
            category.CreatedAt = DateTime.UtcNow;
            category.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spCategoryDelete", category);
                await _logger.SuccessDelete(identity, category.CategoryName, TableName);

                return new ResultDTO<CategorySummaryDTO>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully deleted brand."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailDelete(identity, category.CategoryName, TableName, ex.Message);
                return new ResultDTO<CategorySummaryDTO>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };

            }
        }

        public async Task<CategoryModel?> GetByIdAsync(ClaimsIdentity? identity, int id)
        {
            return await GetByIdAsync<CategoryModel>(identity, "dbo.spCategoryGetById", id);
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await IsItemExistAsync<CategoryModel>("dbo.spCategoryGetById", id);
        }

        private async Task ValidateFields(ClaimsIdentity? identity, CategoryModel category)
        {
            string Name = AppHelper.GetFirstWord(
                identity?.Claims.Where(c => c.Type == ClaimTypes.Name)
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
                await _logger.LogError($"Data Access {Name}: {msg}");
                throw new NullReferenceException(msg);
            }
        }

    }
}
