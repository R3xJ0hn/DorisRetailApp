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
                var result = await _data.AddSubCategoryAsync(GetUserIdentity(),subcategory);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);

            }
            catch (Exception ex)
            {
                await _log.LogError("SubCategoryController[Add]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<SubCategorySummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to add new sub category.",
                        IsSuccessStatusCode = false
                    });
            }
        }

        [HttpPost("update-subcategory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateSubCategories(SubCategoryModel subCategory)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), subCategory.Id);

            if (getExisting == null)
            {
                await _log.LogError("SubCategoryController[Add]: " + GetUserIdentity()?.Name +
                    $" trying to update sub category[{subCategory.Id}] not exist.");
                return BadRequest(
                   new ResultDTO<BrandSummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to update sub category.",
                       IsSuccessStatusCode = false
                   });
            }

            try
            {
                var result = await _data.UpdateSubCategoryAsync(GetUserIdentity(), subCategory);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("SubCategoryController[Update]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to update sub category.",
                        IsSuccessStatusCode = false
                    });
            }
        }

        [HttpPost("get-subcategory/summary")]
        public async Task<IActionResult> GetSubCategorySummary(RequestPageDTO request)
        {
            try
            {
                var result = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("SubCategoryController[Get]: " + ex.Message);
                return BadRequest(
                      new ResultDTO<ProductSummaryDTO>
                      {
                          ErrorCode = 5,
                          ReasonPhrase = "Unable to get categories.",
                          IsSuccessStatusCode = false
                      });
            }
        }

        [HttpPost("delete-subcategory"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteCategories(SubCategoryModel subcategory)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), subcategory.Id);

            if (getExisting == null)
            {
                await _log.LogError("SubCategoryController[Add]: " + GetUserIdentity()?.Name +
                    $"trying to delete sub category[{subcategory.Id}]");
                return BadRequest(
                   new ResultDTO<BrandSummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to delete sub category.",
                       IsSuccessStatusCode = false
                   });
            }

            try
            {
                var result = await _data.DeleteSubCategoryAsync(GetUserIdentity(), subcategory);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result); 
            }
            catch (Exception ex)
            {
               await _log.LogError("SubCategoryController[Delete]: " + ex.Message);
                return BadRequest(
                        new ResultDTO<ProductSummaryDTO>
                        {
                            ErrorCode = 5,
                            ReasonPhrase = "Unable to delete sub category.",
                            IsSuccessStatusCode = false
                        });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSubCategoryById(int id)
        {
            try
            {
                var subCategoryItem = await _data.GetByIdAsync(GetUserIdentity(), id);
                return Ok(
                  new ResultDTO<SubCategoryModel?>
                  {
                      Data = subCategoryItem,
                      ErrorCode = 0,
                      ReasonPhrase = "Unable to get sub category.",
                      IsSuccessStatusCode = false
                  });
            }
            catch (Exception ex)
            {
                await _log.LogError("SubCategoryController[GetById]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to get sub category.",
                        IsSuccessStatusCode = false
                    });
            }
        }

        [HttpGet("get-subcategory/bycategory/{id}")]
        public async Task<IActionResult> GetSubCategorySummaryByCategoryId(int id)
        {
            try
            {
                var subCategoryItems = await _data.GetByCategoryIdAsync(GetUserIdentity(), id);
                return Ok(subCategoryItems);
            }
            catch (Exception ex)
            {
                await _log.LogError("SubCategoryController[GetByCategoryId]: " + ex.Message);
                return BadRequest(
                     new ResultDTO<ProductSummaryDTO>
                     {
                         ErrorCode = 5,
                         ReasonPhrase = "Unable to get sub categories.",
                         IsSuccessStatusCode = false
                     });
            }
        }

    }


}
