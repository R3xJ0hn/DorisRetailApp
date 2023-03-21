using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class BrandEndpoint : BaseEndpoint
    {
        public BrandEndpoint(IAPIHelper apiHelper) 
            : base(apiHelper)
        {
        }

        public async Task AddBrandAsync(BrandModel brand, Stream? imgStream)
        {
            UploadResultDTO? uploadResultDTO = null;

            if (imgStream != null)
            {
                var content = new StreamContent(imgStream);
                var uploadResult = await AddBrandImg(content, brand.ImageName);
                uploadResultDTO = JsonConvert.DeserializeObject<UploadResultDTO>(uploadResult);
            }

            if (uploadResultDTO.Uploaded == true && !string.IsNullOrWhiteSpace(brand.BrandName))
            {
                brand.ImageName = uploadResultDTO.FileName;
                brand.StoredImageName = uploadResultDTO.StoredFileName;
                await SendPostAysnc(brand, "URL:add-brand");
            }
            else
            {
                throw new HttpRequestException("Unable to upload imge.");
            }
        }

        private async Task<string> AddBrandImg(StreamContent content, string fileName)
        {
            var newcontent = new MultipartFormDataContent
            {
                { content, "file", fileName }
            };

           return await SendFilePostAysnc(newcontent);
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
