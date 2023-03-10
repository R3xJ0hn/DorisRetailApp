using DorisApp.WebAPI.DataAccess;
using DorisApp.WebAPI.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : ControllerBase
    {
        private readonly IRoleData _data;

        public RoleController(IRoleData data)
        {
            _data = data;
        }

        [HttpGet]
        [Route("get-roles")]
        public async Task<List<RoleModel>> GetRoles()
        {
            return await _data.GetRolesAsync();
        }

        [HttpGet]
        [Route("get-role/id")]
        public async Task<IActionResult> GetRoleId(int id)
        {
            var role = await _data.GetRoleByIdAsync(id);

            if (role != null)
            {
                return Ok(role);
            }

            return BadRequest("Role not found!");
        }

        [HttpPost("add-role")]
        public IActionResult AddRole(string roleName)
        {
            try
            {
                _data.AddRole(new RoleModel
                {
                    RoleName = roleName,
                }, 1); //TODO 1: replace the login User

                return Ok($"successfully added {roleName} role");
            }
            catch
            {
                return BadRequest("Unable to Add new role.");
            }
        }
    }
}
