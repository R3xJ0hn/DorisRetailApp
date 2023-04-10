using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{
    public class ProductData : BaseDataProcessor
    {
        private readonly BrandData _brandData;
        private readonly CategoryData _categoryData;
        private readonly SubCategoryData _subCategoryData;

        public override string TableName => "Products";

        public ProductData(ISqlDataAccess sql, ILoggerManager logger,
            BrandData brandData, CategoryData categoryData, SubCategoryData subCategoryData) : base(sql, logger)
        {
            _brandData = brandData;
            _categoryData = categoryData;
            _subCategoryData = subCategoryData;
        }

        public async Task<ResultDTO<RequestModel<ProductSummaryDTO>?>> GetSummaryDataByPageAsync(ClaimsIdentity? identity, RequestPageDTO request)
        {
            return await GetByPageAsync<ProductSummaryDTO>(identity, "dbo.spProductGetSummaryByPage", request);
        }

        public async Task<ResultDTO<List<ProductSummaryDTO>>> AddProductAsync(ClaimsIdentity? identity, ProductModel product)
        {
 
            try
            {
                int userId = int.Parse(identity?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "1");
                product.ProductName = AppHelper.CapitalizeFirstWords(product.ProductName);
                product.Size = product.Size.Replace(" ", string.Empty).ToLower();
                product.CreatedByUserId = userId;
                product.UpdatedByUserId = userId;
                product.CreatedAt = DateTime.UtcNow;
                product.UpdatedAt = DateTime.UtcNow;

                if (product.BrandId == 0) product.BrandId = 1;
                if (product.CategoryId == 0) product.CategoryId = 1;
                if (product.SubCategoryId == 0) product.SubCategoryId = 1;

                var getIdentical = await _sql.LoadDataAsync<ProductModel,
                    ProductModel>("spProductGetIdentical", product);

                if (getIdentical.Count > 0)
                {
                    var productModel = getIdentical.FirstOrDefault();

                    //If it is Just mark as deleted
                    if (productModel != null && productModel.ProductName == "*")
                    {
                        productModel.ProductName = product.ProductName;
                        productModel.UpdatedByUserId = userId;
                        productModel.UpdatedAt = DateTime.UtcNow;
                        productModel.StoredImageName = product.StoredImageName;

                        await _sql.UpdateDataAsync("dbo.spProductRestore", productModel);
                        return new ResultDTO<List<ProductSummaryDTO>>
                        {
                            ErrorCode = 0,
                            IsSuccessStatusCode = true,
                            ReasonPhrase = "Successfully restore product."
                        };
                    }

                    //If it is not deleted
                    if (productModel != null && productModel.ProductName != "*")
                    {
                        return new ResultDTO<List<ProductSummaryDTO>>
                        {
                            ErrorCode = 3,
                            IsSuccessStatusCode = false,
                            ReasonPhrase = $"Product not saved: {getIdentical.Count} identical item(s) found."
                        };
                    }

                }

                var errorMsg = await ValidateFields(identity, product);

                if (errorMsg != null)
                {
                    return new ResultDTO<List<ProductSummaryDTO>>
                    {
                        ErrorCode = 1,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = errorMsg
                    };
                }

                await _sql.SaveDataAsync("dbo.spProductInsert", product);
                await _logger.SuccessInsert(identity, product.ProductName, TableName);

                return new ResultDTO<List<ProductSummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully added new product."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailInsert(identity, product.ProductName, TableName, ex.Message);
                return new ResultDTO<List<ProductSummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<List<ProductSummaryDTO>>> UpdateProductAsync(ClaimsIdentity? identity, ProductModel product)
        {
            var oldName = string.Empty;

            try
            {
                var getExistingItem = await GetByIdAsync(identity, product.Id);
                oldName = (await GetByIdAsync(identity, product.Id))?.ProductName;

                if (getExistingItem == null)
                {
                    var msg = $"Product[{product.ProductName}[{product.Id}]] not found.";
                    await _logger.LogError(msg);
                    return new ResultDTO<List<ProductSummaryDTO>>
                    {
                        ErrorCode = 5,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = msg
                    };
                }

                if (string.IsNullOrEmpty(product.StoredImageName))
                {
                    product.StoredImageName = getExistingItem.StoredImageName;
                }

                product.ProductName = AppHelper.CapitalizeFirstWords(product.ProductName);
                product.Size = product.Size.Replace(" ", string.Empty).ToLower();
                product.UpdatedByUserId = int.Parse(identity?.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault() ?? "1");
                product.UpdatedAt = DateTime.UtcNow;

                //This will ignore by the stored procedure.
                product.CreatedAt = DateTime.UtcNow;

                if (product.BrandId == 0 && getExistingItem.BrandId > 0)
                    product.BrandId = getExistingItem.BrandId;

                if (product.CategoryId == 0 && getExistingItem.CategoryId > 0)
                    product.CategoryId = getExistingItem.CategoryId;

                if (product.SubCategoryId == 0 && getExistingItem.SubCategoryId > 0)
                    product.SubCategoryId = getExistingItem.SubCategoryId;

                var errorMsg = await ValidateFields(identity, product);

                if (errorMsg != null)
                {
                    return new ResultDTO<List<ProductSummaryDTO>>
                    {
                        ErrorCode = 1,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = errorMsg
                    };
                }

                await _sql.UpdateDataAsync("dbo.spProductUpdate", product);
                await _logger.SuccessUpdate(identity, product.ProductName, TableName, oldName ?? "");

                return new ResultDTO<List<ProductSummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully update product."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailUpdate(identity, product.ProductName, TableName, oldName ?? "", ex.Message);
                return new ResultDTO<List<ProductSummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<ProductSummaryDTO>> DeleteProductAsync(ClaimsIdentity? identity, ProductModel product)
        {
            product.UpdatedByUserId = int.Parse(identity?.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "1");

            //This will ignore by the stored procedure.
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spProductDelete", product);
                await _logger.SuccessDelete(identity, product.ProductName, TableName);

                return new ResultDTO<ProductSummaryDTO>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully deleted product."
                };
            }
            catch (Exception ex)
            {
                await _logger.FailDelete(identity, product.ProductName, TableName, ex.Message);
                return new ResultDTO<ProductSummaryDTO>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ProductModel?> GetByIdAsync(ClaimsIdentity? identity, int id)
        {
            return await GetByIdAsync<ProductModel>(identity, "dbo.spProductGetById", id);
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await IsItemExistAsync<ProductModel>("dbo.spProductGetById", id);
        }

        public async Task<string?> ValidateFields(ClaimsIdentity? identity, ProductModel product)
        {
            if (identity == null)
            {
                throw new UnauthorizedAccessException();
            }

            string Name = AppHelper.GetFirstWord(
                identity.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");

            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the product.";
            }

            if (string.IsNullOrWhiteSpace(product.ProductName))
            {
                msg = "Product name is null.";
            }

            if (string.IsNullOrWhiteSpace(product.Sku))
            {
                msg = "Product sku is null.";
            }

            if (string.IsNullOrWhiteSpace(product.Size))
            {
                msg = "Product size is null.";
            }

            if (product.BrandId != 0)
            {
                var getBrandIfExist = await _brandData.IsExistAsync(product.BrandId);

                if (!getBrandIfExist)
                {
                    msg = $"Brand[{product.BrandId}] not exist.";
                }
            }

            //Doesn`t need to validate if zero this field is required
            var getCategoryIfExist = await _categoryData.IsExistAsync(product.CategoryId);

            if (!getCategoryIfExist)
            {
                msg = $"Category[{product.CategoryId}] not exist.";
            }

            if (product.SubCategoryId != 0)
            {
                var getSubCategoryIfExist = await _subCategoryData.IsExistAsync(product.SubCategoryId);

                if (!getSubCategoryIfExist)
                {
                    msg = $"SubCategory[{product.SubCategoryId}] not exist.";
                }
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await _logger.LogError($"Data Access {Name}: {msg}");
            }

            return msg;
        }

    }
}
