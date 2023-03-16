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
        public async Task<IActionResult> AddCategory([FromBody] CategoryModel model)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();
            try
            {
                await _data.AddAsync(new CategoryModel
                {
                    CategoryName = model.CategoryName 
                }, int.Parse(userId ?? "0"));

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
            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();

            try
            {
                var categoryItems = await _data.GetTableDataByPageAsync(request, int.Parse(userId ?? "0"));
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
            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();

            try
            {
               await _data.UpdateCategoryAsync(model, int.Parse(userId ?? "0"));
                return Ok($"Successfully update {model.CategoryName}");
            }
            catch (Exception ex)
            {
                return BadRequest("Unable to read categories." + Environment.NewLine + ex.Message);
            }
        }

    }
}
