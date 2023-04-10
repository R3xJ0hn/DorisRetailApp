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
    public class CategoryController : ControllerBase
    {
        private readonly CategoryData _data;
        private readonly ILoggerManager _log;

        public CategoryController(CategoryData data, ILoggerManager log)
        {
            _data = data;
            _log = log;
        }
        private ClaimsIdentity? GetUserIdentity() => (ClaimsIdentity?)User.Identity;

        [HttpPost("add-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddCategory(CategoryModel category)
        {
            try
            {
                var result = await _data.AddCategoryAsync(GetUserIdentity(), category);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("CategoryController[Add]: " + ex.Message);
                return BadRequest(
                new ResultDTO<CategorySummaryDTO>
                {
                    ErrorCode = 5,
                    ReasonPhrase = "Unable to add new category.",
                    IsSuccessStatusCode = false
                });
            }
        }

        [HttpPost("update-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateCategories(CategoryModel category)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), category.Id);

            if (getExisting == null)
            {
                await _log.LogError("CategoryController[Update]: " + GetUserIdentity()?.Name +
                    $"trying to update categoryID[{category.Id}] not exist.");
                return BadRequest(
                   new ResultDTO<CategorySummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to update category.",
                       IsSuccessStatusCode = false
                   });
            }

            try
            {
                var result = await _data.UpdateCategoryAsync(GetUserIdentity(), category);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("CategoryController[Update]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<CategorySummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to update category.",
                        IsSuccessStatusCode = false
                    });
            }
        }


        [HttpPost("get-category/summary")]
        public async Task<IActionResult> GetCategorySummary(RequestPageDTO request)
        {
            try
            {
                var result = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("CategoryController[Get]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<CategorySummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to get categories.",
                        IsSuccessStatusCode = false
                    });
            }
        }


        [HttpPost("delete-category"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(CategoryModel category)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), category.Id);

            if (getExisting == null)
            {
                await _log.LogError("CategoryController[Update]: " + GetUserIdentity()?.Name +
                    $" trying to delete category[{category.Id}] not exist.");
                return BadRequest(
                   new ResultDTO<CategorySummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to delete category.",
                       IsSuccessStatusCode = false
                   });
            }

            try
            {
                var result = await _data.DeleteCategoryAsync(GetUserIdentity(), category);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("CategoryController[Delete]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<CategorySummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to delete category.",
                        IsSuccessStatusCode = false
                    });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var categoryItem = await _data.GetByIdAsync(GetUserIdentity(), id);
                return Ok(
                 new ResultDTO<CategoryModel?>
                 {
                     Data = categoryItem,
                     ErrorCode = 0,
                     ReasonPhrase = "Successfully get category.",
                     IsSuccessStatusCode = false
                 });
            }
            catch (Exception ex)
            {
                await _log.LogError("CategoryController[GetById]: " + ex.Message);
                return BadRequest(
                  new ResultDTO<CategoryModel>
                  {
                      ErrorCode = 5,
                      ReasonPhrase = "Unable to get category." + ex.Message,
                      IsSuccessStatusCode = false
                  });
            }
        }


    }
}
