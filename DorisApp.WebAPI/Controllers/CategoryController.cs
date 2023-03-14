using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryData _data;

        public CategoryController(ICategoryData data)
        {
            _data = data;
        }

        [HttpPost("add-category")]
        public IActionResult AddRole(string categoryName)
        {
            try
            {
                _data.AddNewCategory(new CategoryModel
                {
                    CategoryName = categoryName,
                }, 1); //TODO 1: replace the login User

                return Ok($"successfully added {categoryName} category");
            }
            catch
            {
                return BadRequest("Unable to Add new role.");
            }
        }
    }
}
