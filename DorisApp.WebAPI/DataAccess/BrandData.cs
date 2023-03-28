using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{

    public class BrandData : BaseDataProcessor
    {
        public override string TableName => "Brands";

        public BrandData(ISqlDataAccess sql, ILoggerManager logger) : base(sql, logger)
        {
        }

        public async Task<ResultDTO<RequestModel<BrandSummaryDTO>?>> GetSummaryDataByPageAsync(ClaimsIdentity? identity, RequestPageDTO request)
        {
            return await GetByPageAsync<BrandSummaryDTO>(identity, "dbo.spBrandGetSummaryByPage", request);
        }

        public async Task<ResultDTO<List<BrandSummaryDTO>>> AddBrandAsync(ClaimsIdentity? identity, BrandModel brand)
        {

            try
            {
                int createdByUserId = int.Parse(identity?.Claims.FirstOrDefault(c => 
                        c.Type == ClaimTypes.NameIdentifier)?.Value ?? "1");

                brand.BrandName = AppHelper.CapitalizeFirstWords(brand.BrandName);
                brand.CreatedByUserId = createdByUserId;
                brand.UpdatedByUserId = createdByUserId;
                brand.CreatedAt = DateTime.UtcNow;
                brand.UpdatedAt = brand.CreatedAt;

                var getIdentical = await _sql.LoadDataAsync<BrandModel,
                    BrandModel>("spBrandGetIdentical", brand);

                if (getIdentical.Count > 0)
                {
                    var brandModel = getIdentical.FirstOrDefault();

                    //If it is Just mark as deleted
                    if (brandModel != null && brandModel.BrandName == "*")
                    {
                        brandModel.BrandName = brand.BrandName;
                        brandModel.UpdatedByUserId = createdByUserId;
                        brandModel.UpdatedAt = DateTime.UtcNow;
                        brandModel.StoredImageName = brand.StoredImageName;

                        await _sql.UpdateDataAsync("dbo.spBrandRestore", brandModel);
                        return new ResultDTO<List<BrandSummaryDTO>>
                        {
                            ErrorCode = 0,
                            IsSuccessStatusCode = true,
                            ReasonPhrase = "Successfully restore brand."
                        };
                    }

                    //If it is not deleted
                    if (brandModel != null && brandModel.BrandName != "*")
                    {
                        return new ResultDTO<List<BrandSummaryDTO>>
                        {
                            ErrorCode = 3,
                            IsSuccessStatusCode = false,
                            ReasonPhrase = $"Brand not saved: {getIdentical.Count} identical item(s) found."
                        };
                    }
      
                }

                await ValidateFields(identity, brand);
                await _sql.SaveDataAsync("dbo.spBrandInsert", brand);
                await _logger.SuccessInsert(identity, brand.BrandName, TableName);

                return new ResultDTO<List<BrandSummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully added new brand."
                };
            }
            catch (Exception ex)
            {
                await _logger.FailInsert(identity, brand.BrandName, TableName, ex.Message);
                return new ResultDTO<List<BrandSummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<List<BrandSummaryDTO>>> UpdateBrandAsync(ClaimsIdentity? identity, BrandModel brand)
        {
            var oldName = string.Empty;

            try
            {
                var getExistingItem = await GetByIdAsync(identity, brand.Id);
                oldName = (await GetByIdAsync(identity, brand.Id))?.BrandName;

                if (getExistingItem == null)
                {
                    var msg = $"Brand[{brand.BrandName}[{brand.Id}]] not found.";
                    await _logger.LogError(msg);
                    return new ResultDTO<List<BrandSummaryDTO>>
                    {
                        ErrorCode = 5,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = msg
                    };
                }

                if (string.IsNullOrEmpty(brand.StoredImageName))
                {
                    brand.StoredImageName = getExistingItem.StoredImageName;
                }

                brand.BrandName = AppHelper.CapitalizeFirstWords(brand.BrandName);
                brand.UpdatedByUserId = int.Parse(identity?.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault() ?? "1");
                brand.UpdatedAt = DateTime.UtcNow;

                //This will ignore by the stored procedure.
                brand.CreatedAt = DateTime.UtcNow;

                await ValidateFields(identity, brand);
                await _sql.UpdateDataAsync("dbo.spBrandUpdate", brand);
                await _logger.SuccessUpdate(identity, brand.BrandName, TableName, oldName ?? "");

                return new ResultDTO<List<BrandSummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully update brand."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailUpdate(identity, brand.BrandName, TableName, oldName ?? "", ex.Message);
                return new ResultDTO<List<BrandSummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<BrandSummaryDTO>> DeleteBrandAsync(ClaimsIdentity? identity, BrandModel brand)
        {
            brand.UpdatedByUserId = int.Parse(identity?.Claims
                .Where(c => c.Type == ClaimTypes.NameIdentifier)
                .Select(c => c.Value).SingleOrDefault() ?? "1");

            //This will ignore by the stored procedure.
            brand.CreatedAt = DateTime.UtcNow;
            brand.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _sql.UpdateDataAsync("dbo.spBrandDelete", brand);
                await _logger.SuccessDelete(identity, brand.BrandName, TableName);

                return new ResultDTO<BrandSummaryDTO>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully deleted brand."
                };
            }
            catch (Exception ex)
            {
                await _logger.FailDelete(identity, brand.BrandName, TableName, ex.Message);
                return new ResultDTO<BrandSummaryDTO>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<BrandModel?> GetByIdAsync(ClaimsIdentity? identity, int id)
        {
            return await GetByIdAsync<BrandModel>(identity, "dbo.spBrandGetById", id);
        }

        public async Task<bool> IsExistAsync(int id)
        {
            return await IsItemExistAsync<BrandModel>("dbo.spBrandGetById", id);
        }

        public async Task ValidateFields(ClaimsIdentity? identity, BrandModel brand)
        {
            string Name = AppHelper.GetFirstWord(
                identity?.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");

            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the brand.";
            }

            if (string.IsNullOrEmpty(brand.BrandName))
            {
                msg = $"The Brand name is null.";
            }

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await _logger.LogError($"Data Access {Name}: {msg}");
                throw new NullReferenceException(msg);
            }
        }

    }
}
