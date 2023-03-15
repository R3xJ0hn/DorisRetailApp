using DorisApp.Data.Library.DTO;
using DorisApp.WebAPI.DataAccess.Database;

namespace DorisApp.WebAPI.DataAccess
{
    public abstract class BaseDataProcessor : IDisposable
    {
        protected readonly ISqlDataAccess _sql;
        protected readonly ILogger _logger;

        public abstract string TableName { get; }

        public BaseDataProcessor(ISqlDataAccess sql, ILogger logger)
        {
            _sql = sql;
            _logger = logger;
        }

        public bool ValidateRequestPageDTO(RequestPageDTO request, int totalPage)
        {
            return request != null && request.PageNo > 0 && request.PageNo <= totalPage;
        }

        protected void ErrorPage(RequestPageDTO request)
        {
            string msg = $"Error: Page {request.PageNo} is out of range";
            _logger.LogInformation(msg);
            throw new ArgumentException(msg);
        }

        public async Task<int> CountAsync()
        {
            return await _sql.CountAsync(TableName);
        }

        public void Dispose()
        {
            _sql.Dispose();
        }
    }


}
