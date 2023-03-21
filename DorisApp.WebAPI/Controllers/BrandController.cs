using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
using DorisApp.WebAPI.DataAccess.Logger;
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

        public BrandController(BrandData data,IWebHostEnvironment env , ILoggerManager logger)
        {
            _data = data;
            _env = env;
            _log = logger;
        }

        private ClaimsIdentity GetUserIdentity() => (ClaimsIdentity)User.Identity;


        [HttpPost("add-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBrand(BrandModel brand)
        {
            var fileName = brand.StoredImageName;
            var uploadsFolder = Path.Combine(_env.ContentRootPath, "uploads");
            var tempFolder = Path.Combine(uploadsFolder, "temp");
            var destinationPath = Path.Combine(uploadsFolder, "brand");

            if (fileName != null)
            {
                var targetTempFile = Path.Combine(tempFolder, fileName);

                if (System.IO.File.Exists(targetTempFile))
                {
                    if (!Directory.Exists(destinationPath))
                    {
                        Directory.CreateDirectory(destinationPath);
                    }

                    var path = Path.Combine(destinationPath, fileName);

                    System.IO.File.Move(targetTempFile, path);
                    System.IO.File.Delete(targetTempFile);
                }
            }

            try
            {
                await _data.AddAsync(GetUserIdentity(),
                    new BrandModel
                    {
                        BrandName = brand.BrandName,
                        StoredImageName = fileName,
                    });

                return Ok($"Successfully added {brand.BrandName} category");
            }
            catch { return BadRequest("Unable to add new brand."); }

        }

        [HttpPost("get-brand/summary")]
        public async Task<IActionResult> GetCategorySummary(RequestPageDTO request)
        {
            try
            {
                var categoryItems = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return Ok(categoryItems);
            }
            catch { return BadRequest("Unable to get brand."); }
        }

        [HttpPost("update-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategories(BrandModel brand)
        {
            try
            {
                bool result = await _data.IsExist(brand.Id);
                if (!result)
                { return BadRequest($"Unable to get brand [{brand.Id}]"); }

                await _data.UpdateCategoryAsync(GetUserIdentity(), brand);
                return Ok($"Successfully update {brand.BrandName}");
            }
            catch { return BadRequest("Unable to update brand."); }
        }

        [HttpPost("delete-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(BrandModel brand)
        {
            try
            {
                if (!await _data.IsExist(brand.Id))
                { return BadRequest($"Unable to get brand [{brand.Id}]"); }

                await _data.DeleteCategoryAsync(GetUserIdentity(), brand);
                return Ok($"Successfully remove {brand.BrandName}");
            }
            catch { return BadRequest("Unable to delete brand."); }
        }

    }
}
