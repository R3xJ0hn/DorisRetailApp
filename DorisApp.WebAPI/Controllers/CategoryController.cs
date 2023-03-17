using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
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

        [HttpPost("add-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory(CategoryModel model)
        {
            try
            {
                await _data.AddAsync(new CategoryModel
                {
                    CategoryName = model.CategoryName 
                }, GetUserId());

                return Ok($"Successfully added {model.CategoryName} category");
            }
            catch
            {
                return BadRequest("Unable to Add new role.");
            }
        }

        //TODO: Refactor

        [HttpPost("get-category/summary")]
        public async Task<IActionResult> GetCategorySummary(RequestPageDTO request)
        {
            try
            {
                var categoryItems = await _data.GetSummaryDataByPageAsync(request, GetUserId());
                return Ok(categoryItems);
            }
            catch (Exception ex)
            {
               return BadRequest("Unable to read categories." + Environment.NewLine + ex.Message);
            }
        }

        [HttpPost("update-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategories(CategoryModel model)
        {
            try
            {
               await _data.UpdateCategoryAsync(model, GetUserId());
                return Ok($"Successfully update {model.CategoryName}");
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to update categories." + Environment.NewLine + ex.Message);
            }
        }

        [HttpPost("delete-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(CategoryModel model)
        {
            try
            {
                await _data.DeleteCategoryAsync(model, GetUserId());
                return Ok($"Successfully remove {model.CategoryName}");
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to delete categories." + Environment.NewLine + ex.Message);
            }
        }


        private int GetUserId()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();
            return int.Parse(userId ?? "0");
        }

    }
}
