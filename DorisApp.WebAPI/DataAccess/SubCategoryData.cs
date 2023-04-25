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
        private readonly CategoryData _categoryData;

        public override string TableName => "SubCategories";

        public SubCategoryData(ISqlDataAccess sql, ILoggerManager logger, CategoryData categoryData) 
            : base(sql, logger)
        {
            _categoryData = categoryData;
        }

        public async Task<ResultDTO<RequestModel<SubCategorySummaryDTO>?>> GetSummaryDataByPageAsync(ClaimsIdentity? identity, RequestPageDTO request)
        {
            return await GetByPageAsync<SubCategorySummaryDTO>(identity, "dbo.spSubCategoryGetSummaryByPage", request);
        }

        public async Task<ResultDTO<SubCategoryModel>> AddSubCategoryAsync(ClaimsIdentity? identity, SubCategoryModel subCategory)
        {

            try
            {
                var userId = int.Parse(identity?.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.NameIdentifier)?.Value ?? "1");

                subCategory.SubCategoryName = AppHelper.CapitalizeFirstWords(subCategory.SubCategoryName);
                subCategory.CategoryId = subCategory.CategoryId;
                subCategory.CreatedByUserId = userId;
                subCategory.UpdatedByUserId = subCategory.CreatedByUserId;
                subCategory.CreatedAt = DateTime.UtcNow;
                subCategory.UpdatedAt = DateTime.UtcNow;

                if (subCategory.CategoryId == 0) subCategory.CategoryId = 1;

                var getIdentical = await _sql.LoadDataAsync<SubCategoryModel,
                    SubCategoryModel>("spSubCategoryGetIdentical", subCategory);

                if (getIdentical.Count > 0)
                {
                    var subCategoryModel = getIdentical.FirstOrDefault();

                    //If it is just mark as deleted
                    if (subCategoryModel != null && subCategoryModel.SubCategoryName == "*")
                    {
                        subCategoryModel.SubCategoryName = subCategoryModel.SubCategoryName;
                        subCategoryModel.UpdatedByUserId = userId;
                        subCategoryModel.UpdatedAt = DateTime.UtcNow;

                        await _sql.UpdateDataAsync("dbo.spSubCategoryRestore", subCategoryModel);
                        return new ResultDTO<SubCategoryModel>
                        {
                            Data = subCategoryModel,
                            ErrorCode = 0,
                            IsSuccessStatusCode = true,
                            ReasonPhrase = "Successfully restore category."
                        };
                    }

                    return new ResultDTO<SubCategoryModel>
                    {
                        ErrorCode = 3,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = $"Sub Category not saved: {subCategory.SubCategoryName} Exist!"
                    };
                }

                var errorMsg = await ValidateFields(identity, subCategory);

                if (errorMsg != null)
                {
                    return new ResultDTO<SubCategoryModel>
                    {
                        ErrorCode = 1,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = errorMsg
                    };
                }

                await _sql.SaveDataAsync("dbo.spSubCategoryInsert", subCategory);
                await _logger.SuccessInsert(identity, subCategory.SubCategoryName, TableName);

                return new ResultDTO<SubCategoryModel>
                {
                    Data = subCategory,
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully added new sub category."
                };

            }
            catch (Exception ex)
            {
               await _logger.FailInsert(identity, subCategory.SubCategoryName, TableName, ex.Message);
                return new ResultDTO<SubCategoryModel>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>>> UpdateSubCategoryAsync(ClaimsIdentity? identity, SubCategoryModel subCategory)
        {
            var oldName = string.Empty;

            try
            {
                var getExistingItem = await GetByIdAsync(identity, subCategory.Id);
                oldName = (await GetByIdAsync(identity, subCategory.Id))?.SubCategoryName;

                if (getExistingItem == null)
                {
                    var msg = $"SubCategory[{subCategory.SubCategoryName}[{subCategory.Id}]] not found.";
                    await _logger.LogError(msg);
                    return new ResultDTO<List<SubCategorySummaryDTO>>
                    {
                        ErrorCode = 5,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = msg
                    };
                }

                subCategory.SubCategoryName = AppHelper.CapitalizeFirstWords(subCategory.SubCategoryName);
                subCategory.UpdatedByUserId = int.Parse(identity?.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault() ?? "1");
                subCategory.UpdatedAt = DateTime.UtcNow;

                //This will ignore by the stored procedure.
                subCategory.CreatedAt = DateTime.UtcNow;

                if (subCategory.CategoryId == 0 && getExistingItem.CategoryId > 0)
                    subCategory.CategoryId = getExistingItem.CategoryId;

                var errorMsg = await ValidateFields(identity, subCategory);

                if (errorMsg != null)
                {
                    return new ResultDTO<List<SubCategorySummaryDTO>>
                    {
                        ErrorCode = 1,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = errorMsg
                    };
                }

                await _sql.UpdateDataAsync("dbo.spSubCategoryUpdate", subCategory);
                await _logger.SuccessUpdate(identity, subCategory.SubCategoryName, TableName, oldName ?? "");

                return new ResultDTO<List<SubCategorySummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully update sub category."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailUpdate(identity, subCategory.SubCategoryName, TableName, oldName ?? "", ex.Message);
                return new ResultDTO<List<SubCategorySummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<SubCategorySummaryDTO>> DeleteSubCategoryAsync(ClaimsIdentity? identity, SubCategoryModel subCategory)
        {
            subCategory.UpdatedByUserId = int.Parse(identity?.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "1");

            //This will ignore by the stored procedure.
            subCategory.CreatedAt = DateTime.UtcNow;
            subCategory.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spSubCategoryDelete", subCategory);
                await _logger.SuccessDelete(identity, subCategory.SubCategoryName, TableName);

                return new ResultDTO<SubCategorySummaryDTO>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully deleted sub category."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailDelete(identity, subCategory.SubCategoryName, TableName, ex.Message);
                return new ResultDTO<SubCategorySummaryDTO>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<List<SubCategorySummaryDTO>?>> GetByCategoryIdAsync(ClaimsIdentity? identity, int id)
        {
            try
            {
                var p = new { CategoryId = id };
                var output = await _sql.LoadDataAsync<SubCategorySummaryDTO, dynamic>("dbo.spSubCategoryGetByCategoryId", p);
                await _logger.SuccessRead(identity, TableName, output.Count());

                return new ResultDTO<List<SubCategorySummaryDTO>?>
                {
                    Data = output,
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = $"SubCategory: {output?.Count()} item(s) found."
                };
            }
            catch (Exception ex)
            {
                await _logger.LogError($"Data Access [Get]: {ex.Message}");
                return new ResultDTO<List<SubCategorySummaryDTO>?>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<SubCategoryModel?> GetByIdAsync(ClaimsIdentity? identity, int id)
        {
            return await GetByIdAsync<SubCategoryModel>(identity, "dbo.spSubCategoryGetById", id);
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await IsItemExistAsync<SubCategoryModel>("dbo.spSubCategoryGetById", id);
        }

        private async Task<string?> ValidateFields(ClaimsIdentity? identity, SubCategoryModel subCategory)
        {
            string Name = AppHelper.GetFirstWord(
                identity?.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");

            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the sub category.";
            }

            var getCategoryIfExist = await _categoryData.IsExistAsync(subCategory.CategoryId);

            if (!getCategoryIfExist)
            {
                msg = $"Category[{subCategory.CategoryId}] not exist.";
            }

            if (string.IsNullOrWhiteSpace(subCategory.SubCategoryName))
            {
                msg = $"The Sub Category name in Subcategory is null.";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await _logger.LogError($"Data Access {Name}: {msg}"); 
            }

            return msg;
        }
    }
}
