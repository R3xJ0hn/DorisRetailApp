using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{
    public class InventoryData : BaseDataProcessor
    {
        private readonly ProductData _productData;

        public override string TableName => "Inventory";

        public InventoryData(ISqlDataAccess sql, ILoggerManager logger, ProductData productData) : base(sql, logger)
        {
            _productData = productData;
        }

        public async Task<ResultDTO<RequestModel<InventorySummaryDTO>?>> GetSummaryDataByPageAsync(ClaimsIdentity? identity, RequestInventoryPageDTO request)
        {
            return await GetByPageAsync<InventorySummaryDTO>(identity, "dbo.spInventoryGetSummaryByPage", request);
        }

        public async Task<ResultDTO<List<InventorySummaryDTO>>> StockEntryAsync(ClaimsIdentity? identity, InventoryModel inventory)
        {
            try
            {
                int createdByUserId = int.Parse(identity?.Claims.FirstOrDefault(c =>
                        c.Type == ClaimTypes.NameIdentifier)?.Value ?? "1");

                inventory.Location = inventory.Location.ToUpper();
                inventory.StockRemain = inventory.Quantity;
                inventory.StockAvailable = inventory.Quantity;
                inventory.CreatedByUserId = createdByUserId;
                inventory.UpdatedByUserId = createdByUserId;
                inventory.CreatedAt = DateTime.UtcNow;
                inventory.UpdatedAt = DateTime.UtcNow;

                var errorMsg = await ValidateFields(identity, inventory);

                if (errorMsg != null)
                {
                    return new ResultDTO<List<InventorySummaryDTO>>
                    {
                        ErrorCode = 1,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = errorMsg
                    };
                }

                await _sql.SaveDataAsync("dbo.spInventoryInsert", inventory);
                await _logger.SuccessInsert(identity, inventory.Location, TableName);

                return new ResultDTO<List<InventorySummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = $"Successfully added new product in the inventory."
                };
            }
            catch (Exception ex)
            {
                await _logger.FailInsert(identity, inventory.Location, TableName, ex.Message);
                return new ResultDTO<List<InventorySummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<List<InventorySummaryDTO>>> UpdateInventoryAsync(ClaimsIdentity? identity, InventoryModel inventory)
        {
            var getProduct = await _productData.GetByIdAsync(identity, inventory.ProductId);
            var getExistingItem = await GetByIdAsync(identity, inventory.Id);

            try
            {
                if (getExistingItem == null)
                {
                    var msg = $"Inventory[{inventory.Location}[{getProduct?.ProductName}[{inventory.ProductId}]]] not found.";
                    await _logger.LogError(msg);
                    return new ResultDTO<List<InventorySummaryDTO>>
                    {
                        ErrorCode = 5,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = msg
                    };
                }

                inventory.Location = inventory.Location.ToUpper();
                inventory.UpdatedByUserId = int.Parse(identity?.Claims
                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                    .Select(c => c.Value).SingleOrDefault() ?? "1");
                inventory.UpdatedAt = DateTime.UtcNow;

                var errorMsg = await ValidateFields(identity, inventory);

                if (errorMsg != null)
                {
                    return new ResultDTO<List<InventorySummaryDTO>>
                    {
                        ErrorCode = 1,
                        IsSuccessStatusCode = false,
                        ReasonPhrase = errorMsg
                    };
                }

                await _sql.UpdateDataAsync("dbo.spInventoryUpdate", inventory);
                await _logger.SuccessUpdate(identity,
                    $"{inventory.Location}({getProduct?.ProductName}[QTY[{inventory.Quantity}]])", TableName,
                    $"{inventory.Location}({getProduct?.ProductName}[QTY[{getExistingItem.Quantity}]])");

                return new ResultDTO<List<InventorySummaryDTO>>
                {
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = "Successfully update category."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailUpdate(identity,
                      $"{inventory.Location}({getProduct?.ProductName}[QTY[{inventory.Quantity}]])", TableName,
                      $"{inventory.Location}({getProduct?.ProductName}[QTY[{getExistingItem?.Quantity}]])", ex.Message);
                return new ResultDTO<List<InventorySummaryDTO>>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<ResultDTO<InventoryModel>> ToggleInventory(ClaimsIdentity? identity, InventoryModel inventory)
        {
            var getProduct = await _productData.GetByIdAsync(identity, inventory.ProductId);
            var getExistingItem = await GetByIdAsync(identity, inventory.Id);

            if (getProduct?.IsAvailable == false)
            {
                return new ResultDTO<InventoryModel>
                {
                    ErrorCode = 41,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Product availability is toggled to false, set it true to proceed."
                };
            }

            try
            {
                var data = new { inventory.Id, inventory.IsAvailable };
                var result = await _sql.UpdateDataAsync("dbo.spInventoryToggle", data);
                await _logger.SuccessUpdate(identity,
                    $"{inventory.Location}({getProduct?.ProductName}[Available:{inventory.IsAvailable}])", TableName,
                    $"{inventory.Location}({getProduct?.ProductName}[Available:{getExistingItem?.IsAvailable}])");

                return new ResultDTO<InventoryModel>
                {
                    Data = inventory,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = $"Successfully toggle to {inventory.IsAvailable}"
                };

            }
            catch (Exception ex)
            {
                await _logger.FailUpdate(identity,
                $"{inventory.Location}({getProduct?.ProductName}[Available:{inventory.IsAvailable}])", TableName,
                $"{inventory.Location}({getProduct?.ProductName}[Available:{getExistingItem?.IsAvailable}])",ex.Message);

                return new ResultDTO<InventoryModel>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<InventoryModel?> GetByIdAsync(ClaimsIdentity? identity, int id)
        {
            return await GetByIdAsync<InventoryModel>(identity, "dbo.spInventoryGetById", id);
        }

        public async Task<string?> ValidateFields(ClaimsIdentity? identity, InventoryModel inventory)
        {
            string Name = AppHelper.GetFirstWord(
                identity?.Claims.Where(c => c.Type == ClaimTypes.Name)
                .Select(c => c.Value).SingleOrDefault() ?? "");

            string? msg = null;

            if (string.IsNullOrWhiteSpace(Name))
            {
                msg = $"Unauthorized to modify the brand.";
            }

            var getProductIfExist = await _productData.IsExistAsync(inventory.ProductId);

            if (!getProductIfExist)
            {
                msg = $"Product[{inventory.ProductId}] not exist.";
            }

            if (string.IsNullOrEmpty(inventory.Location))
                msg = $"The Location name is null.";

            if (inventory.PurchasePrice == 0)
                msg = $"The purchase price is zero.";

            if (inventory.RetailPrice == 0)
                msg = $"The retail price is zero.";

            if (inventory.Quantity == 0)
                msg = $"The quantity is zero.";

            if (inventory.ExpiryDate < DateTime.Today.AddDays(3))
                msg = $"The expiration date should be three days after today's date.";

            if (inventory.PurchasedDate.GetHashCode() == 0)
                msg = $"The purchase date is unassigned.";

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await _logger.LogError($"Data Access {Name}: {msg}");
            }

            return msg;
        }
    }
}
