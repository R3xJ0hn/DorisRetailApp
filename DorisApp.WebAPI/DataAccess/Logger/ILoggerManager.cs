using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess.Logger
{
    public interface ILoggerManager
    {
        Task FailDelete(ClaimsIdentity? identity, string propertyName, string tableName, string errrorMessage);
        Task FailInsert(ClaimsIdentity? identity, string propertyName, string tableName, string errorMessage);
        Task FailRead(ClaimsIdentity? identity, string tableName, int count, string errrorMessage);
        Task FailUpdate(ClaimsIdentity? identity, string propertyName, string tableName, string oldName, string errorMessage);
        Task LogError(string message);
        Task LogSaveInfo(string message);
        Task SuccessDelete(ClaimsIdentity? identity, string propertyName, string tableName);
        Task SuccessInsert(ClaimsIdentity? identity, string propertyName, string tableName);
        Task SuccessRead(ClaimsIdentity? identity, string tableName, int count);
        Task SuccessUpdate(ClaimsIdentity? identity, string propertyName, string tableName, string oldName);
    }
}