using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class SalesEndpoint: BaseEndpoint
    {
        public SalesEndpoint(IAPIHelper apiHelper) 
            : base(apiHelper)
        {

        }

        public async Task<ResultDTO<RequestModel<ProductPosDisplayModel>>?> GetProductAvailableAsync(RequestProductPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-sales/products");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <RequestModel<ProductPosDisplayModel>>>(result);
            }
            catch
            {
                return new ResultDTO<RequestModel<ProductPosDisplayModel>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = result
                };
            }
        }
    }
}
