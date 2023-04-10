using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using DorisApp.Data.Library.DTO;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly ProductData _data;
        private readonly IWebHostEnvironment _env;
        private readonly ILoggerManager _log;
        private readonly string rootFolder = "";

        public ProductController(ProductData data, IWebHostEnvironment env, ILoggerManager log)
        {
            _data = data;
            _env = env;
            _log = log;
            rootFolder = _env.ContentRootPath;
        }

        private ClaimsIdentity? GetUserIdentity() => (ClaimsIdentity?)User.Identity;

        [HttpPost("add-product"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddProduct(ProductModel product)
        {
            var fileName = product.StoredImageName;

            try
            {
                var result = await _data.AddProductAsync(GetUserIdentity(), product);

                if (result.IsSuccessStatusCode)
                {
                    AppHelper.MoveTempToDestPath(rootFolder, fileName, "product");
                    return Ok(result);
                }
                else
                {
                    var tempImg = Path.Combine(rootFolder, "uploads", "temp", 
                        product.StoredImageName ?? "");

                    AppHelper.DeleteFile(tempImg);
                    return BadRequest(result);
                }

            }

            catch (Exception ex)
            {
                await _log.LogError("ProductController[Add]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to add new product.",
                        IsSuccessStatusCode = false
                    }
                );
            }
        }

        [HttpPost("update-product"), Authorize(Roles = "admin")]
        public async Task<IActionResult> UpdateProduct(ProductModel product)
        {
            var getExisting = await _data.GetByIdAsync(GetUserIdentity(), product.Id);

            if (getExisting == null)
            {
                await _log.LogError("ProductController[Update]: " + GetUserIdentity()?.Name +
                    $" trying to update brand[{product.Id}] not exist.");
                return BadRequest(
                   new ResultDTO<ProductSummaryDTO>
                   {
                       ErrorCode = 4,
                       ReasonPhrase = "Unable to update product.",
                       IsSuccessStatusCode = false
                   });
            }

            var moved = AppHelper.MoveTempToDestPath(rootFolder, product.StoredImageName, "product");

            try
            {
                if (moved)
                {
                    if (getExisting.StoredImageName != null)
                    {
                        var oldImg = Path.Combine(rootFolder, "uploads",
                        "product", getExisting.StoredImageName);
                        AppHelper.DeleteFile(oldImg);
                    }
                }

                var result = await _data.UpdateProductAsync(GetUserIdentity(), product);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);

            }
            catch (Exception ex)
            {
                await _log.LogError("ProductController[Update]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to update product.",
                        IsSuccessStatusCode = false
                    }
                );
            }
        }

        [HttpPost("get-product/summary")]
        public async Task<IActionResult> GetProductSummary(RequestPageDTO request)
        {
            try
            {
                var result = await _data.GetSummaryDataByPageAsync(GetUserIdentity(), request);
                return result.IsSuccessStatusCode ? Ok(result) : BadRequest(result);
            }
            catch (Exception ex)
            {
                await _log.LogError("ProductController[Get]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to update product.",
                        IsSuccessStatusCode = false
                    }
                );
            }
        }

        [HttpPost("delete-product"), Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteProduct(ProductModel product)
        {
            try
            {
                var getExisting = await _data.GetByIdAsync(GetUserIdentity(), product.Id);

                if (getExisting == null)
                {
                    await _log.LogError("ProductController[Delete]: " + GetUserIdentity()?.Name +
                        $"trying to delete product[{product.Id}]");
                    return BadRequest(
                       new ResultDTO<ProductSummaryDTO>
                       {
                           ErrorCode = 4,
                           ReasonPhrase = "Unable to delete product.",
                           IsSuccessStatusCode = false
                       });
                }

                var result = await _data.DeleteProductAsync(GetUserIdentity(), product);

                if (result.IsSuccessStatusCode)
                {
                    var oldImg = Path.Combine(rootFolder, "uploads",
                    "product", getExisting.StoredImageName ?? "");
                    AppHelper.DeleteFile(oldImg);
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }

            }
            catch (Exception ex)
            {
                await _log.LogError("ProductController[Delete]: " + ex.Message);

                return BadRequest(
                    new ResultDTO<ProductSummaryDTO>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to delete product.",
                        IsSuccessStatusCode = false
                    }
                );
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(int id)
        {
            try
            {
                var productItem = await _data.GetByIdAsync(GetUserIdentity(), id);
                return Ok(
                    new ResultDTO<ProductModel?>
                    {
                        Data = productItem,
                        ErrorCode = 0,
                        ReasonPhrase = "Successfully get product.",
                        IsSuccessStatusCode = false
                    }
                );
            }
            catch (Exception ex)
            {
                await _log.LogError("ProductController[GetById]: " + ex.Message);
                return BadRequest(
                    new ResultDTO<ProductModel>
                    {
                        ErrorCode = 5,
                        ReasonPhrase = "Unable to get product.",
                        IsSuccessStatusCode = false
                    }
                );
            }
        }

    }
}
