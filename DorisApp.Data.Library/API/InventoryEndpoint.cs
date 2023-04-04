using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class InventoryEndpoint : BaseEndpoint
    {
        public InventoryEndpoint(IAPIHelper apiHelper) 
            : base(apiHelper)
        {
        }

        public async Task<int> CountProductItems()
        {
            var countedResult = 0;

            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var sent = await GetInventorySummary(request);

            if (sent?.Data != null)
            {
                var newRequest = new RequestPageDTO
                {
                    PageNo = 1,
                    ItemPerPage = sent.Data.TotalItems
                };

                var getNewRequest = await GetInventorySummary(newRequest);
                countedResult = getNewRequest?.Data.Models.Count ?? 0;
            }

            return countedResult;
        }

        public async Task<ResultDTO<List<InventorySummaryDTO>>?> AddInventoryAsync(InventoryModel inventory)
        {
            await ValidateInventory(inventory);
            var summary = await SendPostAysnc(inventory, "URL:stock-entry");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <List<InventorySummaryDTO>>>(summary);
            }
            catch
            {
                return new ResultDTO<List<InventorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }

        }

        public async Task<ResultDTO<RequestModel<InventorySummaryDTO>>?> GetInventorySummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-inventory/summary");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <RequestModel<InventorySummaryDTO>>>(result);
            }
            catch
            {
                return new ResultDTO<RequestModel<InventorySummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
        }


        private async Task ValidateInventory(InventoryModel inventory)
        {
            var getIfExist = await SendPostByIdAysnc(inventory.ProductId, "URL:get-product/id");

            if (string.IsNullOrEmpty(getIfExist))
                throw new Exception($"Product[{inventory.ProductId}] not exist.");

            if (string.IsNullOrEmpty(inventory.Location))
                throw new Exception("The Location name is null.");

            if (inventory.PurchasePrice == 0)
                throw new Exception("The purchase price is zero.");

            if (inventory.RetailPrice == 0)
                throw new Exception("The retail price is zero.");

            if (inventory.Quantity == 0)
                throw new Exception("The quantity is zero.");

            if (inventory.ExpiryDate < DateTime.UtcNow.AddDays(3))
                throw new Exception("The expiration date should be three days after today's date.");

            if (inventory.PurchaseDate.GetHashCode() == 0)
                throw new Exception("The purchase date is unassigned.");
        }

    }
}
