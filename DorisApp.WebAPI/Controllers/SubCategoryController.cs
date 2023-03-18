using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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
        public async Task<IActionResult> AddSubCategory(SubCategoryModel model)
        {
            try
            {
                await _data.AddAsync(GetUserIdentity(),
                    new SubCategoryModel 
                    {
                        SubCategoryName = model.SubCategoryName,
                        CategoryId= model.CategoryId
                    });

                return Ok($"Successfully added {model.SubCategoryName} category");
            }
            catch { return BadRequest("Unable to Add new role."); }
        }

        [HttpPost("get-subcategory/summary")]
        public async Task<IActionResult> GetSubCategorySummary(RequestPageDTO request)
        {
            try
            {
                var categoryItems = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return Ok(categoryItems);
            }
            catch { return BadRequest("Unable to get categories."); }
        }

        [HttpPost("update-subcategory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSubCategories(SubCategoryModel model)
        {
            try
            {
                await _data.UpdateCategoryAsync(GetUserIdentity(), model);
                return Ok($"Successfully update {model.SubCategoryName}");
            }
            catch { return BadRequest("Unable to update categories."); }
        }

        [HttpPost("delete-subcategory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(SubCategoryModel model)
        {
            try
            {
                await _data.DeleteCategoryAsync(GetUserIdentity(), model);
                return Ok($"Successfully remove {model.SubCategoryName}");
            }
            catch { return BadRequest("Unable to delete categories."); }
        }

    }


}
