using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DorisApp.Data.Library.DTO;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SalesController : ControllerBase
    {
        private readonly SalesData _data;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerManager _log;


        public SalesController(SalesData data, IWebHostEnvironment env, ILoggerManager log)
        {
            _data = data;
            _env = env;
            _log = log;
        }

        private ClaimsIdentity? GetUserIdentity() => (ClaimsIdentity?)User.Identity;

        [HttpPost("get-sales/products")]
        public async Task<IActionResult> GetProductAvailable(RequestPageDTO request)
        {
            try
            {
                var result = await _data.GetProductPosDisplayDataAsync(GetUserIdentity(), request);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("SalesController[Get]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to get products.",
                        IsSuccessStatusCode = false
                    }
                );
            }
        }

    }
}
