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
    public class SubCategoryController : ControllerBase
    {
        private readonly SubCategoryData _data;
        private readonly ILoggerManager _log;

        public SubCategoryController(SubCategoryData data, ILoggerManager log)
        {
            _data = data;
            _log = log;
        }
        private ClaimsIdentity? GetUserIdentity() => (ClaimsIdentity?)User.Identity;


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
            catch (Exception ex)
            {
                _log.LogError("SubCategoryController[Add]: " + ex.Message);
                return BadRequest("Unable to add new sub category.");
            }
        }

        [HttpPost("get-subcategory/summary")]
        public async Task<IActionResult> GetSubCategorySummary(RequestPageDTO request)
        {
            try
            {
                var categoryItems = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return Ok(categoryItems);
            }
            catch (Exception ex)
            {
                _log.LogError("SubCategoryController[Get]: " + ex.Message);
                return BadRequest("Unable to get sub categories.");
            }
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
            catch (Exception ex)
            {
                _log.LogError("SubCategoryController[Update]: " + ex.Message);
                return BadRequest("Unable to update sub category.");
            }
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
            catch (Exception ex)
            {
                _log.LogError("SubCategoryController[Delete]: " + ex.Message);
                return BadRequest("Unable to delete sub category.");
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(int id)
        {
            try
            {
                var subCategoryItem = await _data.GetByIdAsync(GetUserIdentity(), id);
                return Ok(subCategoryItem);
            }
            catch (Exception ex)
            {
                _log.LogError("SubCategoryController[GetById]: " + ex.Message);
                return BadRequest($"Unable to get [{id}] sub category.");
            }
        }

        [HttpGet("get-subcategory/bycategory/{id}")]
        public async Task<IActionResult> GetSubCategorySummaryByCategoryId(int id)
        {
            try
            {
                var categoryItems = await _data.GetByCategoryIdAsync(GetUserIdentity(), id);
                return Ok(categoryItems);
            }
            catch (Exception ex)
            {
                _log.LogError("SubCategoryController[GetByCategoryId]: " + ex.Message);
                return BadRequest($"Unable to get [{id}] sub categories.");
            }
        }

    }


}
