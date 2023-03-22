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
        private readonly string rootFolder = "";

        public BrandController(BrandData data, IWebHostEnvironment env, ILoggerManager log)
        {
            _data = data;
            _env = env;
            _log = log;
            rootFolder = _env.ContentRootPath;
        }

        private ClaimsIdentity GetUserIdentity() => (ClaimsIdentity)User.Identity;

        [HttpPost("add-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBrand(BrandModel brand)
        {
            var fileName = brand.StoredImageName;

            MoveTempToDestPath(rootFolder, fileName, "brand");

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

        public static bool MoveTempToDestPath(string rootFolder, string? soredFileName, string type)
        {
            var uploadsFolder = Path.Combine(rootFolder, "uploads");
            var destinationFolder = Path.Combine(uploadsFolder, type);
            var tempFolder = Path.Combine(uploadsFolder, "temp");

            if (soredFileName == null) return false;

            var targetTempFile = Path.Combine(tempFolder, soredFileName);
            if (!System.IO.File.Exists(targetTempFile)) return false;


            if (!Directory.Exists(destinationFolder))
            {
                Directory.CreateDirectory(destinationFolder);
            }

            var path = Path.Combine(destinationFolder, soredFileName);

            System.IO.File.Move(targetTempFile, path);
            DeleteFile(targetTempFile);

            return true;
        }

        public static bool DeleteFile(string path)
        {
            if (!System.IO.File.Exists(path)) return false;
            System.IO.File.Delete(path);
            return true;
        }


        [HttpPost("update-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategories(BrandModel brand)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), brand.Id);

            if (getExisting == null)
            { return BadRequest($"Unable to get brand [{brand.Id}]"); }

            var moved = MoveTempToDestPath(rootFolder, brand.StoredImageName, "brand");

            try
            {
                if (moved)
                {
                    if (getExisting.StoredImageName != null)
                    {
                        var oldImg = Path.Combine(rootFolder, "uploads",
                        "brand", getExisting.StoredImageName);
                        DeleteFile(oldImg);
                    }
     

                    await _data.UpdateCategoryAsync(GetUserIdentity(), brand);
                }
                else
                {
                    BrandModel justChangeName = new()
                    { 
                        Id = brand.Id,
                        BrandName = brand.BrandName 
                    };
                    await _data.UpdateCategoryAsync(GetUserIdentity(), justChangeName);
                }

                return Ok($"Successfully update {brand.BrandName}");

            }
            catch(Exception ex) 
            {
                _log.LogError(ex.Message);
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
                _log.LogError(ex.Message); return BadRequest("Unable to get brand."); 
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

                await _data.DeleteCategoryAsync(GetUserIdentity(), brand);

                var oldImg = Path.Combine(rootFolder, "uploads",
                            "brand", getExisting.StoredImageName);

                DeleteFile(oldImg);

                return Ok($"Successfully remove {brand.BrandName}");
            }
            catch(Exception ex) 
            {
                _log.LogError(ex.Message);
                return BadRequest("Unable to delete brand.");
            }
        }

    }
}
