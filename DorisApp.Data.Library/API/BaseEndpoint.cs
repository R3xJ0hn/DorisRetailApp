using DorisApp.Data.Library.DTO;
using Newtonsoft.Json;
using System;
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

            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(_apiHelper.Config[urlKey], data);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }

        protected async Task<UploadResultDTO?> SendImg(Stream? stream, string fileName)
        {
            if (stream != null && stream.Length > 0)
            {
                var content = new StreamContent(stream);

                var newcontent = new MultipartFormDataContent
                {
                    { content, "file", fileName }
                };

                var uploadResult = await SendFilePostAysnc(newcontent);
                return JsonConvert.DeserializeObject<UploadResultDTO>(uploadResult);
            }

            return null;
        }

        protected async Task<string> SendFilePostAysnc(HttpContent content)
        {
            using HttpResponseMessage response = await _apiHelper.ApiCleint.PostAsync(_apiHelper.Config["URL:upload-file"], content);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync();
            }
            else
            {
                throw new HttpRequestException(response.ReasonPhrase);
            }
        }

    }

}
