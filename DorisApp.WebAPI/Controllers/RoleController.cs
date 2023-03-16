using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
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


        [HttpGet]
        [Route("get-role/id")]
        public async Task<IActionResult> GetRoleId(int id)
        {
            var role = await _data.GetByIdAsync(id);

            if (role != null)
            {
                return Ok(role);
            }

            return BadRequest("Role not found!");
        }

        [HttpPost("add-role")]
        public async Task<IActionResult> AddRole(string roleName)
        {

            try
            {
               await _data.AddAsync(new RoleModel
                {
                    RoleName = roleName,
                }, GetUserId());

                return Ok($"successfully added {roleName} role");
            }
            catch
            {
                return BadRequest("Unable to Add new role.");
            }
        }

        private int GetUserId()
        {
            var identity = (ClaimsIdentity)User.Identity;
            var userId = identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier)
                   .Select(c => c.Value).SingleOrDefault();
            return int.Parse(userId ?? "0");
        }
    }
}
