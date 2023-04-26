using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess;
using DorisApp.WebAPI.Helpers;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using DorisApp.Data.Library.DTO;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserData _userData;

        public AuthController(UserData userData)
        {
            _userData = userData;
        }

        private ClaimsIdentity? GetUserIdentity() => (ClaimsIdentity?)User.Identity;

        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> Register(RegisterUserModel user)
        {
            try
            {
                var existingUser = await _userData.FindByEmailAsync(user.Email);
                if (existingUser != null)
                {
                    return BadRequest("The email given already exists.");
                }

                var result =  await _userData.RegisterUserAsync(user);
                return Ok($"Welcome {AppHelper.GetFullName(result)}");

            }
            catch (Exception)
            {

                throw;
            }

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] string email, [FromForm] string password)
        {
            var user = await _userData.FindByEmailAsync(email);
            if (user == null)
            {
                return BadRequest("Invalid Username or Password");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return BadRequest("Invalid Username or Password");
            }

            await SetRefreshToken(user);
            string token = await _userData.RequestToken(user);

            return Ok(new
            {
                Access_Token = token,
                Id = user.Id,
                Name = AppHelper.GetFullName(user),
                Email = user.EmailAddress,
                StatusCode= "Authenticated"
            });
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (refreshToken == null)
            {
                return Unauthorized("You must log in first.");
            }

            var userId = refreshToken.Split('.')[1];
            var user = await _userData.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest($"User Not found: {userId}");
            }

            if (user.Token != refreshToken)
            {
                return Unauthorized("Invalid Refresh Token.");
            }

            if (user.TokenExpires < DateTime.UtcNow)
            {
                return Unauthorized("Token expired.");
            }

            string token = await _userData.RequestToken(user);
            await SetRefreshToken(user);
            return Ok(token);
        }


        [HttpPost("request-stamp")]
        public async Task<IActionResult> RequestSecurityStamp([FromForm] string password)
        {
            int userId = int.Parse(GetUserIdentity()?.Claims.FirstOrDefault(c =>
                    c.Type == ClaimTypes.NameIdentifier)?.Value ?? "1");
            var user = await _userData.FindByIdAsync(userId.ToString());

            var hash = "";

            if (!BCrypt.Net.BCrypt.Verify(password, user?.PasswordHash))
            {
                return BadRequest(
                new ResultDTO<string>
                {
                    ErrorCode = 4,
                    ReasonPhrase = "Invalid Password.",
                    IsSuccessStatusCode = false
                });
            }
            else
            {
                int minute = (DateTime.Now.Minute / 10) * 10;
                var stamp = DateTime.UtcNow.ToString("MddyyyH") + minute + userId.ToString();
                hash = BCrypt.Net.BCrypt.HashPassword(stamp);
            }

            return Ok(
            new ResultDTO<string>
            {
                Data = hash,
                ReasonPhrase = "Successfully request new stamp",
                IsSuccessStatusCode = true
            });
        }

        private async Task SetRefreshToken(UserModel user)
        {
            //Update Token in UserModel
            await _userData.SetGenerateRefreshToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = user.TokenExpires,
            };

            Response.Cookies.Append("refreshToken", user.Token, cookieOptions);
        }
    }
}
