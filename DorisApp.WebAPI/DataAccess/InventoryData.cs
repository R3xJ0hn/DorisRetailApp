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

        public async Task<ResultDTO<List<InventorySummaryDTO>>> StockEntryAsync(ClaimsIdentity? identity, InventoryModel inventory)
        {
            try
            {
                int createdByUserId = int.Parse(identity?.Claims.FirstOrDefault(c =>
                        c.Type == ClaimTypes.NameIdentifier)?.Value ?? "1");

                inventory.Location = AppHelper.CapitalizeFirstWords(inventory.Location);
                inventory.CreatedByUserId = createdByUserId;
                inventory.UpdatedByUserId= createdByUserId;
                inventory.CreatedAt= DateTime.UtcNow;
                inventory.UpdatedAt= DateTime.UtcNow;

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

            if (inventory.PurchaseDate.GetHashCode() == 0 )
                msg = $"The purchase date is unassigned.";

            if (!string.IsNullOrWhiteSpace(msg))
            {
                await _logger.LogError($"Data Access {Name}: {msg}");
            }

            return msg;
        }
    }
}
