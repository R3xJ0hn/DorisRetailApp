using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using DorisApp.WebAPI.Helpers;
using System.Security.Claims;

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

        protected async Task<T?> GetByIdAsync<T>(ClaimsIdentity identity, string storeProcedureName, int modelId) where T : class 
        {
            try
            {
                var p = new { Id = modelId };
                var output = await _sql.LoadDataAsync<T, dynamic>(storeProcedureName, p);
                _logger.SuccessRead(identity, TableName, output.Count);
                return output.FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.FailRead(identity, TableName, 0,ex.Message);
                throw;
            }
        }

        protected async Task<RequestModel<T>?> GetByPageAsync<T>(ClaimsIdentity identity, string storeProcedureName, RequestPageDTO request) where T : class
        {
            if (request == null)
            {
                _logger.FailRead(identity,TableName, 0, $"sent {typeof(T).Name} is null");
                throw new ArgumentNullException(nameof(request));
            }

            if (storeProcedureName == null)
            {
                _logger.FailRead(identity, TableName, 0, $"storeProcedureName is null");
                throw new ArgumentNullException(nameof(storeProcedureName));
            }

            if (request.ItemPerPage == 0)
            {
                _logger.FailRead(identity, TableName, 0, $"Item per page is 0");
                throw new ArgumentNullException(nameof(request.ItemPerPage));
            }

            var count = await CountAsync(identity);
            var countPages = AppHelper.CountPages(count, request.ItemPerPage);

            if (!ValidateRequestPageDTO(request, countPages))
            {
                _logger.FailRead(identity, TableName, 0, $"Page {request.PageNo} is out of range: total pages {countPages}");
                return null;
            }

            try
            {
                var output = await _sql.LoadDataAsync<T, RequestPageDTO>(storeProcedureName, request);
                _logger.SuccessRead(identity, TableName, output.Count);

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
               _logger.FailRead(identity,TableName,0,ex.Message);
                throw;
            }
        }

        public async Task<int> CountAsync(ClaimsIdentity identity)
        {
            try
            {
                return await _sql.CountAsync(TableName);
            }
            catch (Exception ex)
            {
                _logger.FailRead(identity,TableName,0, ex.Message);
                throw;
            }
        }

        public bool ValidateRequestPageDTO(RequestPageDTO request, int totalPage)
        {
            return request != null && request.PageNo > 0 && request.PageNo <= totalPage;
        }

        public void Dispose()
        {
            _sql.Dispose();
        }
    }

}
