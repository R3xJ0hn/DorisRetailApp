﻿using DorisApp.WebAPI.DataAccess;
using DorisApp.WebAPI.Helpers;
using DorisApp.WebAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IUserData _userData;
        private readonly IRoleData _roleData;

        public AuthController(IConfiguration config, IUserData userData, IRoleData roleData)
        {
            _config = config;
            _userData = userData;
            _roleData = roleData;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> Register(string firstName, string lastName, string email, string password)
        {
            var existingUser = await _userData.FindByEmailAsync(email);
            if (existingUser != null)
            {
                return BadRequest("The email given already exists.");
            }

            var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
            var user = new UserModel
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = email,
                PasswordHash = passwordHash,
                LastPasswordHash = passwordHash
            };

            var createdUser = await _userData.RegisterUserAsync(user);
            return Ok($"Welcome {StringHelpers.GetFullName(createdUser)}");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
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

            SetRefreshToken(user);

            var token = CreateToken(user, await GetUserRoleName(user));
            return Ok(token);
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

            string token = CreateToken(user, await GetUserRoleName(user));
            SetRefreshToken(user);
            return Ok(token);
        }

        private string CreateToken(UserModel user, string? roleName)
        {
            var tkey = _config["JWTConfig:Key"];
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(tkey);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                new Claim(ClaimTypes.Name, StringHelpers.GetFullName(user)),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Role, roleName),
            }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                                         SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private void SetRefreshToken(UserModel user)
        {
            //Update Token in UserModel
            _userData.SetGenerateRefreshToken(user);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = user.TokenExpires,
            };

            Response.Cookies.Append("refreshToken", user.Token, cookieOptions);
        }

        private async Task<string?> GetUserRoleName(UserModel user)
        {
            return (await _roleData.GetRoleByIdAsync(user.RoleId))?.RoleName;
        }
    }
}