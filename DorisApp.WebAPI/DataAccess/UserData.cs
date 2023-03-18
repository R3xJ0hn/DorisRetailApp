using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DorisApp.WebAPI.DataAccess
{
    public class UserData : BaseDataProcessor
    {
        private readonly RoleData _roleData;
        private readonly IConfiguration _config;

        public override string TableName => "Users";

        public UserData(ISqlDataAccess sql, ILoggerManager logger,
            IConfiguration config, RoleData roleData) : base(sql, logger)
        {
            _roleData = roleData;
            _config = config;
        }

        public async Task<UserModel> RegisterUserAsync(RegisterUserModel user)
        {
            var passwordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            var getRole = _roleData.GetIdForNewUser();

            if (!ValidateEmail(user.Email))
            {
                _logger.LogError($"Invalid email: {user.Email}");
                throw new Exception($"Invalid email: {user.Email}");
            }

            if (getRole <= 0)
            {
                _logger.LogError("Role id not found.");
                throw new Exception("Role id not found.");
            }

            var newUser = new UserModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                EmailAddress = user.Email,
            };

            foreach (var prop in user.GetType().GetProperties())
            {
                if (prop.PropertyType == typeof(string))
                {
                    var value = (string)prop.GetValue(user);
                    if (!string.IsNullOrEmpty(value))
                    {
                        prop.SetValue(user, value.ToLower());
                    }
                }
            }

            newUser.PasswordHash = passwordHash;
            newUser.LastPasswordHash = passwordHash;
            newUser.LastPasswordCanged = DateTime.UtcNow;
            newUser.CreatedAt = DateTime.UtcNow;
            newUser.UpdatedAt = DateTime.UtcNow;
            newUser.RoleId = getRole;
            await SetGenerateRefreshToken(newUser);

            try
            {
                await _sql.SaveDataAsync("dbo.spUserInsert", newUser);
                _logger.LogInfo($"Welcome {AppHelper.GetFullName(newUser)}");
                return newUser;
            }
            catch (Exception ex)
            {
                _logger.LogError("Unable to register User:" + Environment.NewLine + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<UserModel?> FindByIdAsync(string id)
        {
            var parameters = new { Id = id };
            var users = await _sql.LoadDataAsync<UserModel, dynamic>("dbo.spUserGetById", parameters);
            var user = users.FirstOrDefault();
            return user;
        }

        public async Task<UserModel?> FindByEmailAsync(string email)
        {
            if (!ValidateEmail(email))
            {
                return null;
            }

            var parameters = new { Email = email };
            var users = await _sql.LoadDataAsync<UserModel, dynamic>("dbo.spUserGetByEmail", parameters);
            var user = users.FirstOrDefault();
            return user;
        }

        public async Task SetGenerateRefreshToken(UserModel user)
        {
            var generateToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var tokenExpires = DateTime.UtcNow.AddDays(double.Parse(_config["AppSettings:TokenExpire"]));

            generateToken += "." + user.Id.ToString();

            user.Token = generateToken;
            user.TokenCreated = DateTime.UtcNow;
            user.TokenExpires = tokenExpires;

            try
            {
                await _sql.UpdateDataAsync<UserModel>("dbo.spUserUpdateToken", user);
                _logger.LogInfo($"Seccessfully request {AppHelper.GetFullName(user)} for new request token.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Fail {AppHelper.GetFullName(user)} " +
                    $"to for new request token." +
                    Environment.NewLine + ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<string> RequestToken(UserModel user)
        {
            var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, AppHelper.GetFullName(user)) }, "Request Token");

            var a = identity;

            var role = (await _roleData.GetByIdAsync(identity, user.RoleId)).RoleName;

            return CreateToken(user,role);
        }

        private string CreateToken(UserModel user, string? roleName)
        {
            var key = Encoding.UTF8.GetBytes(_config["JwtConfig:Key"]);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature);

            var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, AppHelper.GetFullName(user)),
                    new Claim(ClaimTypes.Role, roleName),
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Nbf, new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds().ToString()),
                    new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(DateTime.Now.AddDays(1)).ToUnixTimeSeconds().ToString())
                };

            var token = new JwtSecurityTokenHandler().WriteToken(new JwtSecurityToken(
                new JwtHeader(credentials),
                new JwtPayload(claims)));

            return token;
        }

        private bool ValidateEmail(string email)
        {
            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);
            return emailRegex.IsMatch(email);
        }

        public void Dispose()
        {
            _sql.Dispose();
        }
    }

}
