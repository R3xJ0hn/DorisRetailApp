using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryController : ControllerBase
    {
        private readonly InventoryData _data;
        private readonly ILoggerManager _log;

        public InventoryController(InventoryData data, ILoggerManager log)
        {
            _data = data;
            _log = log;
        }

        private ClaimsIdentity? GetUserIdentity() => (ClaimsIdentity?)User.Identity;

        [HttpPost("stock-entry"), Authorize(Roles = "admin")]
        public async Task<IActionResult> StockEntry(InventoryModel inventory)
        {
            try
            {
                var result = await _data.StockEntryAsync(GetUserIdentity(), inventory);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("InventoryController[Add]: " + ex.Message);
                return BadRequest(
                new ResultDTO<InventorySummaryDTO>
                {
                    ErrorCode = 5,
                    ReasonPhrase = "Unable to entry new product in the inventory.",
                    IsSuccessStatusCode = false
                });
            }
        }

        [HttpPost("get-inventory/summary")]
        public async Task<IActionResult> GetInventorySummary(RequestInventoryPageDTO request)
        {
            try
            {
                var result = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("InventoryController[Get]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<InventorySummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to get inventories.",
                        IsSuccessStatusCode = false
                    });
            }
        }

        [HttpPost("update-inventory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateInventory(InventoryModel inventory)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), inventory.Id);

            int minute = (DateTime.Now.Minute / 10) * 10;
            var stamp = DateTime.UtcNow.ToString("MddyyyH") + minute + 
                    GetUserIdentity()?.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(inventory.SecurityStamp))
            {
                return BadRequest(
                  new ResultDTO<InventorySummaryDTO>
                  {
                      ErrorCode = 41,
                      ReasonPhrase = "Please provide a valid stamp",
                      IsSuccessStatusCode = false
                  });
            }

            bool isValid = BCrypt.Net.BCrypt.Verify(stamp, inventory.SecurityStamp);

            if (!isValid)
            {
                return BadRequest(
                  new ResultDTO<InventorySummaryDTO>
                  {
                      ErrorCode = 41,
                      ReasonPhrase = "Invalid Security Stamp.",
                      IsSuccessStatusCode = false
                  });
            }

            if (getExisting == null)
            {
                await _log.LogError("InventoryController[Update]: " + GetUserIdentity()?.Name +
                    $"trying to update product inventory in {inventory.Location} not exist.");

                return BadRequest(
                   new ResultDTO<InventorySummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to update inventory.",
                       IsSuccessStatusCode = false
                   });
            }

            try
            {
                var result = await _data.UpdateInventoryAsync(GetUserIdentity(), inventory);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("InventoryController[Update]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<CategorySummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to update inventory.",
                        IsSuccessStatusCode = false
                    });
            }
        }

        [HttpPost("toggle-inventory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> InventoryToggle(InventoryModel inventory)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), inventory.Id);

            if (getExisting == null)
            {
                await _log.LogError("InventoryController[Update]: " + GetUserIdentity()?.Name +
                    $"trying to update product inventory in {inventory.Location} not exist.");

                return BadRequest(
                   new ResultDTO<InventorySummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to update inventory.",
                       IsSuccessStatusCode = false
                   });
            }

            try
            {
                var result = await _data.ToggleInventory(GetUserIdentity(), inventory);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("InventoryController[Update]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<CategorySummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to toggle inventory.",
                        IsSuccessStatusCode = false
                    });
            }
        }






        [HttpGet("{id}")]
        public async Task<IActionResult> GetinventoryById(int id)
        {
            try
            {
                var inventoryItem = await _data.GetByIdAsync(GetUserIdentity(), id);
                return Ok(
                 new ResultDTO<InventoryModel?>
                 {
                     Data = inventoryItem,
                     ErrorCode = 0,
                     ReasonPhrase = "Successfully get inventory.",
                     IsSuccessStatusCode = false
                 });
            }
            catch (Exception ex)
            {
                await _log.LogError("InventoryController[GetById]: " + ex.Message);
                return BadRequest(
                  new ResultDTO<InventoryModel>
                  {
                      ErrorCode = 5,
                      ReasonPhrase = "Unable to get category." + ex.Message,
                      IsSuccessStatusCode = false
                  });
            }
        }
    }
}
