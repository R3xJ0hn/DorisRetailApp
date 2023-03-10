using DorisApp.WebAPI.DataAccess;
using DorisApp.WebAPI.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text;

namespace DorisApp.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserData _userData;

        public AuthController(IUserData userData)
        {
            _userData = userData;
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
            return Ok($"Welcome {GetFullName(createdUser)}");
        }

        private static string GetFullName(UserModel user)
        {
            var firstName = char.ToUpper(user.FirstName[0]) + user.FirstName[1..];
            var lastName = char.ToUpper(user.LastName[0]) + user.LastName[1..];
            return $"{CapitalizeWords(firstName)} {CapitalizeWords(lastName)}";
        }

        private static string CapitalizeWords(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
                return value;

            StringBuilder result = new(value);
            result[0] = char.ToUpper(result[0]);

            for (int i = 1; i < result.Length; ++i)
            {
                if (char.IsWhiteSpace(result[i - 1]))
                    result[i] = char.ToUpper(result[i]);
            }

            return result.ToString();
        }

    }
}
