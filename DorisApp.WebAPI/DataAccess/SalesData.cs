using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.DataAccess.Logger;
using System.Security.Claims;

namespace DorisApp.WebAPI.DataAccess
{
    public class SalesData : BaseDataProcessor
    {
        private string _tableName = "Sales";
        public override string TableName => _tableName;

        public SalesData(ISqlDataAccess sql, ILoggerManager logger)
            : base(sql, logger)
        {
        }

        public async Task<ResultDTO<RequestModel<ProductPosDisplayModel>?>> GetProductPosDisplayDataAsync(ClaimsIdentity? identity, RequestPageDTO request)
        {
            _tableName = "Inventory"; // We need to temporarily modify the _tableName value based on the number of products in the inventory.
            var result = await GetByPageAsync<ProductPosDisplayModel>(identity, "dbo.spSalesGetAvailableProducts", request);
            _tableName = "Sales";

            return result;  
        }

    }
}
