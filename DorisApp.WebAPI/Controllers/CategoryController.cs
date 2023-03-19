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
    public class CategoryController : ControllerBase
    {
        private readonly CategoryData _data;

        public CategoryController(CategoryData data)
        {
            _data = data;
        }
        private ClaimsIdentity GetUserIdentity() => (ClaimsIdentity)User.Identity;


        [HttpPost("add-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory(CategoryModel category)
        {
            try
            {
                await _data.AddAsync(GetUserIdentity(),
                    new CategoryModel { CategoryName = category.CategoryName });

                return Ok($"Successfully added {category.CategoryName} category");
            }
            catch { return BadRequest("Unable to add new category."); }
        }

        [HttpPost("get-category/summary")]
        public async Task<IActionResult> GetCategorySummary(RequestPageDTO request)
        {
            try
            {
                var categoryItems = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return Ok(categoryItems);
            }
            catch { return BadRequest("Unable to get categories."); }
        }

        [HttpPost("update-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategories(CategoryModel category)
        {
            try
            {
                if (!await _data.IsExist(category.Id))
                { return BadRequest($"Unable to get category [{category.Id}]"); }

                await _data.UpdateCategoryAsync(GetUserIdentity(), category);
                return Ok($"Successfully update {category.CategoryName}");
            }
            catch { return BadRequest("Unable to update categories."); }
        }

        [HttpPost("delete-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(CategoryModel category)
        {
            try
            {
                if (!await _data.IsExist(category.Id))
                { return BadRequest($"Unable to get category [{category.Id}]"); }

                await _data.DeleteCategoryAsync(GetUserIdentity(), category);
                return Ok($"Successfully remove {category.CategoryName}");
            }
            catch { return BadRequest("Unable to delete categories."); }
        }


    }
}
