using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
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

        public BrandController(BrandData data)
        {
            _data = data;
        }
        private ClaimsIdentity GetUserIdentity() => (ClaimsIdentity)User.Identity;


        [HttpPost("add-brand"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddBrand(BrandModel brand)
        {
            try
            {
                await _data.AddAsync(GetUserIdentity(),
                    new BrandModel 
                    {
                        BrandName= brand.BrandName,
                        ThumbnailName= brand.ThumbnailName,
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
