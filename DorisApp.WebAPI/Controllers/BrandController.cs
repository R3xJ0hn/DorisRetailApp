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

            AppHelper.MoveTempToDestPath(rootFolder, fileName, "brand");

            try
            {
                await _data.AddBrandAsync(GetUserIdentity(),
                    new BrandModel
                    {
                        BrandName = brand.BrandName,
                        StoredImageName = fileName,
                    });

                return Ok($"Successfully added {brand.BrandName} category");
            }

            catch (Exception ex)
            {
                _log.LogError("BrandController[Add]: " + ex.Message);
                return BadRequest("Unable to add new brand.");
            }
        }


        [HttpPost("update-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategories(BrandModel brand)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), brand.Id);

            if (getExisting == null)
            { return BadRequest($"Unable to get brand [{brand.Id}]"); }

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

                    await _data.UpdateBrandAsync(GetUserIdentity(), brand);
                }
                else
                {
                    BrandModel justChangeName = new()
                    {
                        Id = brand.Id,
                        BrandName = brand.BrandName
                    };
                    await _data.UpdateBrandAsync(GetUserIdentity(), justChangeName);
                }

                return Ok($"Successfully update {brand.BrandName}");

            }
            catch (Exception ex)
            {
                _log.LogError("BrandController[Update]: " + ex.Message);
                return BadRequest("Unable to update brand.");
            }
        }


        [HttpPost("get-brand/summary")]
        public async Task<IActionResult> GetCategorySummary(RequestPageDTO request)
        {
            try
            {
                var categoryItems = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return Ok(categoryItems);
            }
            catch (Exception ex)
            {
                _log.LogError("BrandController[Get]: " + ex.Message);
                return BadRequest("Unable to get brands.");
            }
        }

        [HttpPost("delete-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(BrandModel brand)
        {
            try
            {
                var getExisting = await _data.GetByIdAsync(GetUserIdentity(), brand.Id);

                if (getExisting == null)
                { return BadRequest($"Unable to get brand [{brand.Id}]"); }

                await _data.DeleteBrandAsync(GetUserIdentity(), brand);

                var oldImg = Path.Combine(rootFolder, "uploads",
                            "brand", getExisting.StoredImageName);

                AppHelper.DeleteFile(oldImg);

                return Ok($"Successfully remove {brand.BrandName}");
            }
            catch (Exception ex)
            {
                _log.LogError("BrandController[Delete]: " + ex.Message);
                return BadRequest("Unable to delete brand.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBrandById(int id)
        {
            try
            {
                var brandItem = await _data.GetByIdAsync(GetUserIdentity(), id);
                return Ok(brandItem);
            }
            catch (Exception ex)
            {
                _log.LogError("BrandController[GetById]: " + ex.Message);
                return BadRequest($"Unable to get [{id}] brand.");
            }
        }

    }
}
