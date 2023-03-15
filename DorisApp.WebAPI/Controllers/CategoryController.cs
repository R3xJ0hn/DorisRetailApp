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
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryData _data;

        public CategoryController(ICategoryData data)
        {
            _data = data;
        }

        //TODO:(Roles = "admin")
        [HttpPost("add-category"), Authorize(Roles = "admin")]
        public IActionResult AddCategory([FromBody] CategoryModel model)
        {
            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();
            try
            {
                _data.AddNewCategory(new CategoryModel
                {
                    CategoryName = model.CategoryName 
                }, int.Parse(userId) ); //TODO 1: replace the login User

                return Ok($"Successfully added {model.CategoryName} category");
            }
            catch
            {
               
                return BadRequest("Unable to Add new role.");
            }
        }
    }
}
