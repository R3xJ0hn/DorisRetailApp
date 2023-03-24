using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DorisApp.WebAPI.DataAccess
{

    public abstract class BaseDataProcessor : IDisposable
    {
        protected readonly ISqlDataAccess _sql;
        protected readonly ILoggerManager _logger;
        public abstract string TableName { get; }

        public BaseDataProcessor(ISqlDataAccess sql, ILoggerManager logger)
        {
            _sql = sql;
            _logger = logger;
        }

        protected async Task<T?> GetByIdAsync<T>(ClaimsIdentity? identity, string storeProcedureName, int modelId) where T : class
        {
            try
            {
                var p = new { Id = modelId };
                var output = await _sql.LoadDataAsync<T, dynamic>(storeProcedureName, p);
                await _logger.SuccessRead(identity, TableName, output.Count);
                return output.FirstOrDefault();
            }
            catch (Exception ex)
            {
                await _logger.FailRead(identity, TableName, 0, ex.Message);
                throw;
            }
        }

        protected async Task<ResultDTO<RequestModel<T>?>> GetByPageAsync<T>(ClaimsIdentity? identity, string storeProcedureName, RequestPageDTO request) where T : class
        {
            if (request == null)
            {
                var msg = $"sent {typeof(T).Name} is null";
                await _logger.FailRead(identity, TableName, 0, msg);
                return new ResultDTO<RequestModel<T>?>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = msg
                };
            }

            if (storeProcedureName == null)
            {
                var msg = $"storeProcedureName is null";
                await _logger.FailRead(identity, TableName, 0, msg);
                return new ResultDTO<RequestModel<T>?>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = msg
                };
            }

            if (request.ItemPerPage == 0)
            {
                var msg = $"Item per page is 0";
                await _logger.FailRead(identity, TableName, 0, msg);
                return new ResultDTO<RequestModel<T>?>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = msg
                };
            }

            var count = await CountAsync(identity);
            var countPages = AppHelper.CountPages(count, request.ItemPerPage);

            if (!ValidateRequestPageDTO(request, countPages))
            {
                var msg = $"Page {request.PageNo} is out of range: total pages {countPages}";
                await _logger.FailRead(identity, TableName, 0, msg);
                return new ResultDTO<RequestModel<T>?>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = msg
                };
            }

            try
            {
                var output = await _sql.LoadDataAsync<T, RequestPageDTO>(storeProcedureName, request);
                await _logger.SuccessRead(identity, TableName, output.Count);

                var data = new RequestModel<T>
                {
                    Models = output,
                    IsInPage = request.PageNo,
                    ItemPerPage = request.ItemPerPage,
                    TotalPages = countPages,
                    TotalItems = count
                };

                return new ResultDTO<RequestModel<T>?>
                {
                    Data = data,
                    ErrorCode = 0,
                    IsSuccessStatusCode = true,
                    ReasonPhrase = $"Product: {data?.TotalItems} item(s) found."
                };

            }
            catch (Exception ex)
            {
                await _logger.FailRead(identity, TableName, 0, ex.Message);
                return new ResultDTO<RequestModel<T>?>
                {
                    ErrorCode = 5,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = "Server error."
                };
            }
        }

        public async Task<int> CountAsync(ClaimsIdentity? identity)
        {
            try
            {
                return await _sql.CountAsync(TableName);
            }
            catch (Exception ex)
            {
                await _logger.FailRead(identity, TableName, 0, ex.Message);
                throw;
            }
        }

        private static bool ValidateRequestPageDTO(RequestPageDTO request, int totalPage)
        {
            return request != null && request.PageNo > 0 && request.PageNo <= totalPage;
        }

        protected async Task<bool> IsItemExistAsync<T>(string storeProcedureName, int id) where T : class
        {
            var p = new { Id = id };
            var exist = await _sql.LoadDataAsync<T, dynamic>(storeProcedureName, p);
            return exist.Count != 0;
        }

        public void Dispose() => _sql.Dispose();
    }

}
