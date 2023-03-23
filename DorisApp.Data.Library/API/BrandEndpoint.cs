using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class BrandEndpoint : BaseEndpoint
    {
        public BrandEndpoint(IAPIHelper apiHelper)
            : base(apiHelper)
        {
        }

        public async Task<int> CountBrandItems()
        {
            var request = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = 1
            };

            var result = await GetBrandSummary(request);
            return result != null ? result.TotalItems : 0;
        }

        public async Task AddBrandAsync(BrandModel brand, Stream? imgStream, string fileName)
        {
            ValidateBrand(brand);

            if (imgStream != null && imgStream.Length != 0)
            {
                var result = await SendImg(imgStream, fileName);

                if (result != null)
                {
                    brand.StoredImageName = result.StoredFileName;
                }
            }

            await SendPostAysnc(brand, "URL:add-brand");
        }

        public async Task<string> UpdateBrand(BrandModel brand, Stream? imgStream, string fileName)
        {
            ValidateBrand(brand);

            if (imgStream != null && imgStream.Length != 0)
            {
                var result = await SendImg(imgStream, fileName);

                if (result != null)
                {
                    brand.StoredImageName = result.StoredFileName;
                }
            }

            return await SendPostAysnc(brand, "URL:update-brand");
        }

        public async Task<RequestModel<BrandSummaryDTO>?> GetBrandSummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-brand/summary");
            return JsonConvert.DeserializeObject<RequestModel<BrandSummaryDTO>>(result);
        }

        public async Task<string> DeleteBrand(BrandModel model)
        {
            return await SendPostAysnc(model, "URL:delete-brand");
        }

        private void ValidateBrand(BrandModel brand)
        {
            if (string.IsNullOrWhiteSpace(brand.BrandName))
                throw new NullReferenceException("Brand name is null.");

           
        }
    }
}
