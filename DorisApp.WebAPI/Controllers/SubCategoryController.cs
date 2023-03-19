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
    public class SubCategoryController : ControllerBase
    {
        private readonly SubCategoryData _data;

        public SubCategoryController(SubCategoryData data)
        {
            _data = data;
        }
        private ClaimsIdentity GetUserIdentity() => (ClaimsIdentity)User.Identity;


        [HttpPost("add-subcategory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddSubCategory(SubCategoryModel subcategory)
        {
            try
            {
                await _data.AddAsync(GetUserIdentity(),
                    new SubCategoryModel 
                    {
                        SubCategoryName = subcategory.SubCategoryName,
                        CategoryId= subcategory.CategoryId
                    });

                return Ok($"Successfully added {subcategory.SubCategoryName} category");
            }
            catch { return BadRequest("Unable to add new sub category."); }
        }

        [HttpPost("get-subcategory/summary")]
        public async Task<IActionResult> GetSubCategorySummary(RequestPageDTO request)
        {
            try
            {
                var categoryItems = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return Ok(categoryItems);
            }
            catch { return BadRequest("Unable to get sub categories."); }
        }

        [HttpPost("update-subcategory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSubCategories(SubCategoryModel subcategory)
        {
            try
            {
                if (!await _data.IsExist(subcategory.Id))
                { return BadRequest($"Unable to get sub category [{subcategory.Id}]"); }

                await _data.UpdateCategoryAsync(GetUserIdentity(), subcategory);
                return Ok($"Successfully update {subcategory.SubCategoryName}");
            }
            catch { return BadRequest("Unable to update sub categories."); }
        }

        [HttpPost("delete-subcategory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(SubCategoryModel subcategory)
        {
            try
            {
                if (!await _data.IsExist(subcategory.Id))
                { return BadRequest($"Unable to get sub category [{subcategory.Id}]"); }

                await _data.DeleteCategoryAsync(GetUserIdentity(), subcategory);
                return Ok($"Successfully remove {subcategory.SubCategoryName}");
            }
            catch { return BadRequest("Unable to delete sub categories."); }
        }

    }


}
