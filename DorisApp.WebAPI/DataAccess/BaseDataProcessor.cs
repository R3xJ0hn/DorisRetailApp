using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.Helpers;

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
        protected async Task<RequestModel<T>?> GetByPageAsync<T>(string storeProcedureName, RequestPageDTO request, int userId) where T : class
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (storeProcedureName == null)
            {
                throw new ArgumentNullException(nameof(storeProcedureName));
            }

            var count = await CountAsync();
            var countPages = AppHelpers.CountPages(count, request.ItemPerPage);

            if (!ValidateRequestPageDTO(request, countPages))
            {
                ErrorPage(request);
                return null;
            }

            try
            {
                var output = await _sql.LoadDataAsync<T, RequestPageDTO>(storeProcedureName, request);
                _logger.LogInformation($"Success: Get {typeof(T).Name} count:{output.Count} by {userId} at {DateTime.UtcNow}");

                return new RequestModel<T>
                {
                    Models = output,
                    IsInPage = request.PageNo,
                    ItemPerPage = request.ItemPerPage,
                    TotalPages = countPages,
                    TotalItems = count
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error: Get {typeof(T).Name} by {userId} at {DateTime.UtcNow}" + Environment.NewLine + ex.Message);
                throw;
            }
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
            try
            {
                return await _sql.CountAsync(TableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        public void Dispose()
        {
            _sql.Dispose();
        }
    }


}
