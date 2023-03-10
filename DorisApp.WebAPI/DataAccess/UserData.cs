using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.Model;
using System.Security.Cryptography;
using System.Text.RegularExpressions;

namespace DorisApp.WebAPI.DataAccess
{
    public class UserData : IDisposable, IUserData
    {
        private readonly IConfiguration _config;
        private readonly ISqlDataAccess _sql;
        private readonly ILogger<UserData> _logger;
        private readonly IRoleData _roleData;

        public UserData(IConfiguration config, ISqlDataAccess sql, ILogger<UserData> logger, IRoleData roleData)
        {
            _config = config;
            _sql = sql;
            _logger = logger;
            _roleData = roleData;
        }

        public async Task<UserModel> RegisterUserAsync(UserModel user)
        {
            user.RoleId = _roleData.GetNewUserId();
            var getRole = await _roleData.GetRoleByIdAsync(user.RoleId);
            var passwordHash = user.PasswordHash;

            if (!ValidateEmail(user.EmailAddress))
            {
                throw new ArgumentException($"Invalid email: {user.EmailAddress}");
            }

            if (getRole == null)
            {
                _logger.LogError($"Role id not found: {user.RoleId}");
                throw new ArgumentException("Role id not found.");
            }

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

            user.PasswordHash = passwordHash;
            user.LastPasswordHash = passwordHash;
            user.LastPasswordCanged = DateTime.UtcNow;
            user.CreatedAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            SetGenerateRefreshToken(ref user);

            try
            {
                await _sql.SaveDataAsync("dbo.spUserInsert", user);
                _logger.LogInformation("Insert User {@User}", user);
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Insert User {@User}", user);
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<UserModel?> FindByIdAsync(string id)
        {
            var parameters = new { Id = id };
            var users = await _sql.LoadDataAsync<UserModel, dynamic>("dbo.spUserGetById", parameters);
            var user = users.FirstOrDefault();
            _logger.LogInformation("Found user {UserId}", user?.Id);
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
            _logger.LogInformation("Found user {UserEmail}", user?.EmailAddress);
            return user;
        }

        public void SetGenerateRefreshToken(ref UserModel user)
        {
            var generateToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
            var tokenExpires = DateTime.UtcNow.AddDays(double.Parse(_config["AppSettings:TokenExpire"]));

            generateToken += "." + user.Id.ToString();

            user.Token = generateToken;
            user.TokenCreated = DateTime.UtcNow;
            user.TokenExpires = tokenExpires;
            //TODO 2: Update User In the Database
        }

        private bool ValidateEmail(string email)
        {
            var emailRegex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$", RegexOptions.IgnoreCase);

            if (!emailRegex.IsMatch(email))
            {
                _logger.LogError("Invalid email format: {Email}", email);
                return false;
            }

            return true;
        }

        public void Dispose()
        {
            _sql.Dispose();
        }
    }

}
