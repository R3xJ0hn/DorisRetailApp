using DorisApp.Data.Library.DTO;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class BaseEndpoint
    {
        private readonly IAPIHelper _apiHelper;

        public BaseEndpoint(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        protected async Task<string> SendPostAysnc<T>(T model, string urlKey)
        {
            var json = JsonConvert.SerializeObject(model);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var url = _apiHelper.Config[urlKey];

            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(url, data);

            var result = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(result))
            {
                var x = new ResultDTO<string>
                {
                    Data = result,
                    ReasonPhrase = response.ReasonPhrase,
                    ErrorCode = (int)response.StatusCode,
                };

                return JsonConvert.SerializeObject(x);
            }
            return result ;
        }

        protected async Task<string> SendPostByIdAysnc(int id, string urlKey)
        {
            using HttpResponseMessage response = await _apiHelper
                .ApiCleint.GetAsync(_apiHelper.Config[urlKey] + "/" + id);
            return await response.Content.ReadAsStringAsync();
        }

        protected async Task<UploadResultDTO?> SendImgAsync(Stream? stream, string? fileName)
        {
            if (stream != null && stream.Length > 0)
            {
                stream.Position = 0; 
                var content = new StreamContent(stream);

                var newcontent = new MultipartFormDataContent
                {
                    { content, "file", fileName }
                };

                var uploadResult = await SendFilePostAysnc(newcontent);

                try
                {
                    return JsonConvert.DeserializeObject<UploadResultDTO>(uploadResult);
                }
                catch
                {
                    return null;
                }
            }
            return null;
        }

        protected async Task<string> SendFilePostAysnc(HttpContent content)
        {
            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(_apiHelper.Config["URL:upload-file"], content);
            return await response.Content.ReadAsStringAsync();
        }

    }
}