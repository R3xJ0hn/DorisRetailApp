using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Schema;
using System;
using System.Collections.Generic;
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

            var sent = await GetBrandSummary(request);

            var newRequest = new RequestPageDTO
            {
                PageNo = 1,
                ItemPerPage = sent.Data.TotalItems 
            };

            var result = await GetBrandSummary(newRequest);

            return result?.Data.Models.Count ?? 0;
        }

        public async Task<ResultDTO<List<BrandSummaryDTO>>?> AddBrandAsync(BrandModel brand, Stream? imgStream, string? fileName)
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

            var summary = await SendPostAysnc(brand, "URL:add-brand");

            try
            {
                return JsonConvert.DeserializeObject<ResultDTO
                    <List<BrandSummaryDTO>>>(summary);
            }
            catch 
            {
                return new ResultDTO<List<BrandSummaryDTO>>
                {
                    ErrorCode = 4,
                    IsSuccessStatusCode = false,
                    ReasonPhrase = summary
                };
            }

           

        }

        public async Task<ResultDTO<List<BrandSummaryDTO>>?> UpdateBrand(BrandModel brand, Stream? imgStream, string? fileName)
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

            var summary = await SendPostAysnc(brand, "URL:update-brand");
            return JsonConvert.DeserializeObject<ResultDTO<List<BrandSummaryDTO>>>(summary);
        }

        public async Task<ResultDTO<RequestModel<BrandSummaryDTO>>?> GetBrandSummary(RequestPageDTO request)
        {
            var result = await SendPostAysnc(request, "URL:get-brand/summary");
            return JsonConvert.DeserializeObject<ResultDTO<RequestModel<BrandSummaryDTO>>>(result);
        }

        public async Task<ResultDTO<BrandSummaryDTO>?> DeleteBrand(BrandModel model)
        {
            var result = await SendPostAysnc(model, "URL:delete-brand");
            return JsonConvert.DeserializeObject<ResultDTO<BrandSummaryDTO>>(result);
        }

        private void ValidateBrand(BrandModel brand)
        {
            if (string.IsNullOrWhiteSpace(brand.BrandName))
                throw new NullReferenceException("Brand name is null.");
        }

    }
}
