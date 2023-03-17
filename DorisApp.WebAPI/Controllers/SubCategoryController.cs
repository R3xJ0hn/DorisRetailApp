using DorisApp.Data.Library.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubCategoryController : ControllerBase
    {
    }

    //[HttpPost("add-category"), Authorize(Roles = "admin")]
    //public async Task<IActionResult> AddSubCategory(CategoryModel model)
    //{

    //}
}
