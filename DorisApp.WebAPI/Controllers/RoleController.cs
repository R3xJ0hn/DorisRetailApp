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
    public class RoleController : ControllerBase
    {
        private readonly RoleData _data;

        public RoleController(RoleData data)
        {
            _data = data;
        }
        private ClaimsIdentity GetUserIdentity() => (ClaimsIdentity)User.Identity;


        [HttpGet]
        [Route("get-role/id"), Authorize(Roles = "admin")]
        public async Task<IActionResult> GetRoleId(int id)
        {
            var role = await _data.GetByIdAsync(GetUserIdentity(), id);

            if (role != null)
            {
                return Ok(role);
            }

            return BadRequest("Role not found!");
        }

        [HttpPost("add-role"), Authorize(Roles = "admin")]
        public async Task<IActionResult> AddRole(string roleName)
        {
            try
            {
               await _data.AddAsync(GetUserIdentity(),
                    new RoleModel { RoleName = roleName });

                return Ok($"Successfully added {roleName} role");
            }
            catch
            {
                return BadRequest("Unable to Add new role.");
            }
        }

    }
}
