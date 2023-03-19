using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class BrandEndpoint : BaseEndpoint
    {
        public BrandEndpoint(IAPIHelper apiHelper) 
            : base(apiHelper)
        {
        }

        public async Task AddBrandAsync(string brandName, string imgName)
        {
            var category = new BrandModel()
            {
                BrandName = brandName,
                ThumbnailName = imgName
            };
            await SendPostAysnc(category, "URL:add-brand");
        }

        public async Task<RequestModel<BrandSummaryDTO>?> GetBrandSummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-brand/summary");
            return JsonConvert.DeserializeObject<RequestModel<BrandSummaryDTO>>(result);
        }

        public async Task<string> UpdateBrand(BrandModel model)
        {
            return await SendPostAysnc(model, "URL:update-brand");
        }

        public async Task<string> DeleteBrand(BrandModel model)
        {
            return await SendPostAysnc(model, "URL:delete-brand");
        }
    }
}
