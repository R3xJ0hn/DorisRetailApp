using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess.Logger
{
    public interface ILoggerManager
    {
        void FailDelete(ClaimsIdentity identity, string propertyName, string tableName, string errrorMessage);
        void FailInsert(ClaimsIdentity identity, string propertyName, string tableName, string errorMessage);
        void FailRead(ClaimsIdentity identity, string tableName, int count, string errrorMessage);
        void FailUpdate(ClaimsIdentity identity, string propertyName, string tableName, string oldName, string errorMessage);
        void LogError(string message);
        void LogInfo(string message);
        void SuccessDelete(ClaimsIdentity identity, string propertyName, string tableName);
        void SuccessInsert(ClaimsIdentity identity, string propertyName, string tableName);
        void SuccessRead(ClaimsIdentity identity, string tableName, int count);
        void SuccessUpdate(ClaimsIdentity identity, string propertyName, string tableName, string oldName);
    }
}