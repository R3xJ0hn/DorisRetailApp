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








    }
}
