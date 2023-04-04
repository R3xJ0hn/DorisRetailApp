using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BrandController : ControllerBase
    {
        private readonly BrandData _data;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerManager _log;
        private readonly string rootFolder = "";

        public BrandController(BrandData data, IWebHostEnvironment env, ILoggerManager log)
        {
            _data = data;
            _env = env;
            _log = log;
            rootFolder = _env.ContentRootPath;
        }

        private ClaimsIdentity? GetUserIdentity() => (ClaimsIdentity?)User.Identity;

        [HttpPost("add-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBrand(BrandModel brand)
        {
            var fileName = brand.StoredImageName;

            try
            {
                var result = await _data.AddBrandAsync(GetUserIdentity(), brand);

                if (result.IsSuccessStatusCode)
                {
                    AppHelper.MoveTempToDestPath(rootFolder, fileName, "brand");
                    return Ok(result);
                }
                else
                {
                    var tempImg = Path.Combine(rootFolder, "uploads", "temp",
                        brand.StoredImageName ?? "");

                    AppHelper.DeleteFile(tempImg);
                    return BadRequest(result);
                }

            }

            catch (Exception ex)
            {
                await _log.LogError("BrandController[Add]: " + ex.Message);
                return BadRequest(
                new ResultDTO<BrandSummaryDTO>
                {
                    ErrorCode = 5,
                    ReasonPhrase = "Unable to add new product.",
                    IsSuccessStatusCode = false
                });
            }
        }


        [HttpPost("update-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateBrand(BrandModel brand)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), brand.Id);

            if (getExisting == null)
            {
                await _log.LogError("ProductController[Add]: " + GetUserIdentity()?.Name +
                    $" trying to update brandID[{brand.Id}] not exist.");
                return BadRequest(
                   new ResultDTO<BrandSummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to update brand.",
                       IsSuccessStatusCode = false
                   });
            }

            var moved = AppHelper.MoveTempToDestPath(rootFolder, brand.StoredImageName, "brand");

            try
            {
                if (moved)
                {
                    if (getExisting.StoredImageName != null)
                    {
                        var oldImg = Path.Combine(rootFolder, "uploads",
                        "brand", getExisting.StoredImageName);
                        AppHelper.DeleteFile(oldImg);
                    }

                    var result = await _data.UpdateBrandAsync(GetUserIdentity(), brand);
                    return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
                }
                else
                {
                    BrandModel justChangeName = new()
                    {
                        Id = brand.Id,
                        BrandName = brand.BrandName
                    };

                    var result = await _data.UpdateBrandAsync(GetUserIdentity(), justChangeName);
                    return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                await _log.LogError("BrandController[Update]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to update brand.",
                        IsSuccessStatusCode = false
                    });
            }
        }

        [HttpPost("get-brand/summary")]
        public async Task<IActionResult> GetBrandSummary(RequestPageDTO request)
        {
            try
            {
                var result = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("BrandController[Get]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to get brands.",
                        IsSuccessStatusCode = false
                    });
            }
        }


        [HttpPost("delete-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteBrand(BrandModel brand)
        {
            try
            {
                var getExisting = await _data.GetByIdAsync(GetUserIdentity(), brand.Id);

                if (getExisting == null)
                {
                    await _log.LogError("BrandController[Add]: " + GetUserIdentity()?.Name +
                        $" trying to delete brand[{brand.Id}] not exist.");
                    return BadRequest(
                       new ResultDTO<BrandSummaryDTO>
                       {
                           ErrorCode = 4,
                           ReasonPhrase = "Unable to delete brand.",
                           IsSuccessStatusCode = false
                       });
                }

                var result = await _data.DeleteBrandAsync(GetUserIdentity(), brand);

                if (result.IsSuccessStatusCode)
                {
                    var oldImg = Path.Combine(rootFolder, "uploads",
                    "brand", getExisting.StoredImageName ?? "");
                    AppHelper.DeleteFile(oldImg);
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                await _log.LogError("BrandController[Delete]: " + ex.Message);

                return BadRequest(
                    new ResultDTO<BrandSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to delete brand.",
                        IsSuccessStatusCode = false
                    }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            try
            {
                var brandItem = await _data.GetByIdAsync(GetUserIdentity(), id);
                return Ok(
                    new ResultDTO<BrandModel?>
                    {
                        Data = brandItem,
                        ErrorCode = 0,
                        ReasonPhrase = "Successfully get brand.",
                        IsSuccessStatusCode = false
                    }
                );
            }
            catch (Exception ex)
            {
                await _log.LogError("BrandController[GetById]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to get brand.",
                        IsSuccessStatusCode = false
                    });
            }
        }

    }
}
