using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess.Logger
{
    public class LoggerManager : ILoggerManager
    {
        private readonly ILogger _logger;
        private readonly ISqlDataAccess _sql;

        public LoggerManager(ILogger logger, ISqlDataAccess sql)
        {
            _logger = logger;
            _sql = sql;
        }

        private async Task Log(ClaimsIdentity? identity, string action, string noun, string tableName, string? oldName = null, string? searchFor = null, int count = 0, string? errorExept = null, int status = 0)
        {
            var userId = int.Parse(identity?.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault() ?? "0");
            var userName = identity?.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault() ?? "anonymous";

            var deviceName = "";
            var date = DateTime.UtcNow;
            var firstName = AppHelper.GetFirstWord(userName);

            //User[identity] successfullyAdded[action]  oatmeal[noun] Product[tableName]
            //User[identity]    failToAdd[action]       oatmeal[noun] Product[tableName]

            //User[identity] successfullyUpdate[action] oatmeal[noun] Product[tableName] to OutMeal[newName]
            //User[identity]    failUpdate[action]      oatmeal[noun] Product[tableName] to OutMeal[newName] 

            //User[identity]      read[action]          oatmeal[noun] Product[tableName] 
            //User[identity]    failToRead[action]      oatmeal[noun] Product[tableName]

            //User[identity] successfullyRemove[action] oatmeal[noun] Product[tableName]
            //User[identity]    failToRemove[action]    oatmeal[noun] Product[tableName]

            //User[identity]     searchFor[action]         oat%[noun] in Product[tableName] 10[count]
            //User[identity]     successfully get[action]  Product[tableName]  10[count] 

            var message = firstName;

            //[Action]
            if (!string.IsNullOrEmpty(searchFor))
            {
                //The search for is using the Read() so we
                //need to detect if the searchFor has a value.
                message += $" search for '{searchFor}' in ";
            }
            else
            {
                message += $" {action} ";
            }

            if (noun != "e29h2oeks2#%wdw79yiuh6ada^&^57")
            {
                message += $"{noun} ";
            }

            message += $"{tableName.ToLower()} ";

            if (!string.IsNullOrEmpty(oldName))
            {
                message += $"from {oldName} to {noun}";
            }

            if (count != 0)
            {
                message += $"count[{count}]";
            }

            if (status == 1 || status == 3 || status == 5)
            {

                ActivityLogModel log = new()
                {
                    Message = message,
                    CreatedByUserId = userId,
                    Username = userName,
                    Device = "",
                    Location = "",
                    StatusCode = status,
                    CreatedAt = date,
                };

                await _sql.SaveDataAsync("dbo.spActivityLogInsert", log);
            }


            var msg = $"{message}: [userId:{userId}] [device:{deviceName}] [date:{date}]";

            if (errorExept == null)
            {
                _logger.LogInformation(msg);
            }
            else
            {
                //TODO: Log to the Json File
                _logger.LogError(errorExept, msg);
            }

        }

        public async Task SuccessInsert(ClaimsIdentity? identity, string propertyName, string tableName)
        {
            await Log(identity, "Successfully added", propertyName, tableName, null, null, 0, null, 1);
        }

        public async Task FailInsert(ClaimsIdentity? identity, string propertyName, string tableName, string errorMessage)
        {
            await Log(identity, "Fail to add", propertyName, tableName, null, null, 0, errorMessage, 2);
        }

        public async Task SuccessUpdate(ClaimsIdentity? identity, string propertyName, string tableName, string oldName)
        {
            await Log(identity, "Successfully update", propertyName, tableName, oldName, null, 0, null, 3);
        }

        public async Task FailUpdate(ClaimsIdentity? identity, string propertyName, string tableName, string oldName, string errorMessage)
        {
            await Log(identity, "Fail to update", propertyName, tableName, oldName, null, 0, errorMessage, 4);
        }

        public async Task SuccessDelete(ClaimsIdentity? identity, string propertyName, string tableName)
        {
            await Log(identity, "Successfully deleted", propertyName, tableName, null, null, 0, null, 5);
        }
        public async Task FailDelete(ClaimsIdentity? identity, string propertyName, string tableName, string errrorMessage)
        {
            await Log(identity, "Fail to delete", propertyName, tableName, null, null, 0, errrorMessage, 6);
        }

        public async Task SuccessRead(ClaimsIdentity? identity, string tableName, int count)
        {
            await Log(identity, "Successfully get", "e29h2oeks2#%wdw79yiuh6ada^&^57", tableName, null, null, count, null, 7);
        }

        public async Task FailRead(ClaimsIdentity? identity, string tableName, int count, string errrorMessage)
        {
            await Log(identity, "Fail to get", "e29h2oeks2#%wdw79yiuh6ada^&^57", tableName, null, null, count, errrorMessage, 8);
        }

        public async Task LogSaveInfo(string message)
        {
            ActivityLogModel log = new()
            {
                Message = message,
                CreatedByUserId = 1,
                Username = "",
                Device = "",
                Location = "",
                StatusCode = 9,
                CreatedAt = DateTime.UtcNow,
            };

            await _sql.SaveDataAsync("dbo.spActivityLogInsert", log);
            _logger.LogError(message + ": statusCode[9]");
        }

        public async Task LogError(string message)
        {
            //TODO: Log to the Json File
            _logger.LogError(message + ": statusCode[10]");
        }
    }

}
