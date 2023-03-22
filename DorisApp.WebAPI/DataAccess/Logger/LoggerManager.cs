using DorisApp.WebAPI.Helpers;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace DorisApp.WebAPI.DataAccess.Logger
{
    public class LoggerManager : ILoggerManager
    {
        private readonly ILogger _logger;

        public LoggerManager(ILogger logger)
        {
            _logger = logger;
        }

        private void Log(ClaimsIdentity identity, string action, string noun, string tableName, string? oldName = null, string? searchFor = null, int count = 0, string? errorExept = null)
        {
            var userId = int.Parse(identity.Claims.Where(c => c.Type == ClaimTypes.NameIdentifier).Select(c => c.Value).SingleOrDefault() ?? "0");
            var userName = identity.Claims.Where(c => c.Type == ClaimTypes.Name).Select(c => c.Value).SingleOrDefault() ?? "anonymous";

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

            //TODO: Save this in the database
            //ActivityLogModel log = new()
            //{
            //    Message = message,
            //    UserId = userId,
            //    Username = userName,
            //    Date = date,
            //};

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

        public void SuccessInsert(ClaimsIdentity identity, string propertyName, string tableName)
        {
            Log(identity, "Successfully added", propertyName, tableName, null, null, 0);
        }

        public void FailInsert(ClaimsIdentity identity, string propertyName, string tableName, string errorMessage)
        {
            Log(identity, "Fail to add", propertyName, tableName, null, null, 0, errorMessage);
        }

        public void SuccessUpdate(ClaimsIdentity identity, string propertyName, string tableName, string oldName)
        {
            Log(identity, "Successfully update", propertyName, tableName, oldName, null, 0);
        }

        public void FailUpdate(ClaimsIdentity identity, string propertyName, string tableName, string oldName, string errorMessage)
        {
            Log(identity, "Fail to update", propertyName, tableName, oldName, null, 0, errorMessage);
        }

        public void SuccessDelete(ClaimsIdentity identity, string propertyName, string tableName)
        {
            Log(identity, "Successfully deleted", propertyName, tableName, null, null, 0);
        }
        public void FailDelete(ClaimsIdentity identity, string propertyName, string tableName, string errrorMessage)
        {
            Log(identity, "Fail to delete", propertyName, tableName, null, null, 0, errrorMessage);
        }

        public void SuccessRead(ClaimsIdentity identity, string tableName, int count)
        {
            Log(identity, "Successfully get", "e29h2oeks2#%wdw79yiuh6ada^&^57", tableName, null, null, count);
        }

        public void FailRead(ClaimsIdentity identity, string tableName, int count, string errrorMessage)
        {
            Log(identity, "Fail to get", "e29h2oeks2#%wdw79yiuh6ada^&^57", tableName, null, null, count, errrorMessage);
        }

        public void LogError(string message)
        {
            //TODO: Log to the Json File
            _logger.LogError(message);
        }

        public void LogInfo(string message)
        {
            //TODO: Log to the database
            _logger.LogError(message);
        }

    }

}
