using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using DorisApp.WebAPI.DataAccess.Database;
using DorisApp.WebAPI.Helpers;

namespace DorisApp.WebAPI.DataAccess
{
    public class RoleData: BaseDataProcessor
    {
        public override string TableName => "Roles";

        public RoleData(ISqlDataAccess sql, ILogger logger) 
            : base(sql, logger)
        {
        }
        public async Task AddAsync(RoleModel role, int userId)
        {
            try
            {
                role.RoleName = role.RoleName.ToLower();
                role.CreatedByUserId = userId;
                role.UpdatedByUserId = role.CreatedByUserId;
                role.CreatedAt = DateTime.UtcNow;
                role.UpdatedAt = role.CreatedAt;

               await _sql.SaveDataAsync("dbo.spRoleInsert", role);
                _logger.LogInformation($"Success: Insert {role.RoleName} Category by {userId} at {role.CreatedAt}");
            }

            catch (Exception ex)
            {
                _logger.LogInformation($"Error: Insert {role.RoleName} Category by {userId} at {role.CreatedAt} {ex.Message}");
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<RequestModel<RoleModel>?> GetByPageAsync(RequestPageDTO request)
        {
            try
            {
                var count = await CountAsync();
                var countPages = AppHelpers.CountPages(count, request.ItemPerPage);

                if (ValidateRequestPageDTO(request, countPages))
                {
                    var output = await _sql.LoadDataAsync<RoleModel, RequestPageDTO>("dbo.spRoleGetByPage", request);
                    _logger.LogInformation($"Success: Get Role count:{output.Count} at {DateTime.UtcNow}");

                    RequestModel<RoleModel> reqResult = new()
                    {
                        Models = output,
                        IsInPage = request.PageNo,
                        ItemPerPage = request.ItemPerPage,
                        TotalPages = countPages,
                        TotalItems = count
                    };

                    return reqResult;
                }

                ErrorPage(request);
                return null;
            }
            catch (Exception ex)
            {
                throw new ArgumentException(ex.Message);
            }
        }

        public async Task<RoleModel?> GetByIdAsync(int id)
        {
            var p = new { Id = id };
            var output = await _sql.LoadDataAsync<RoleModel, dynamic>("dbo.spRoleGetById", p);
            _logger.LogInformation($"Success: Get Role with RoleId count:{output.Count} at {DateTime.UtcNow}");
            return output.FirstOrDefault();
        }

        public int GetNewUserId()
        {
            return 1;
        }


    }
}
