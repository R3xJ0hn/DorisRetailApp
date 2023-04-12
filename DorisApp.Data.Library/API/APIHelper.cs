using DorisApp.Data.Library.DTO;
using DorisApp.Data.Library.Model;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace DorisApp.Data.Library.API
{
    public class APIHelper : IDisposable, IAPIHelper
    {
        private readonly HttpClient _apiClient;
        private readonly IConfiguration _config;
        public HttpClient ApiCleint => _apiClient;
        public IConfiguration Config => _config;

        public APIHelper(IConfiguration config)
        {
            _config = config;

            _apiClient = new HttpClient
            {
                BaseAddress = new Uri(_config[key: "URL:apiUrl"]),
                Timeout = TimeSpan.FromSeconds(int.Parse(_config[key: "AppSettings:timeOutSec"] ?? "5"))
            };
        }

        public async Task<string?> LogInUserAsync(string username, string password)
        {
            var data = new FormUrlEncodedContent(new[]
{
                new KeyValuePair<string, string>("email", username),
                new KeyValuePair<string, string>("password", password)
            });

            var apiURL = _config[key: "URL:apiUrl"] + _config[key: "URL:login"];
            var authResult = await _apiClient.PostAsync(requestUri: apiURL, data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            var obj = JsonConvert.DeserializeObject<AuthenticatedUserModel>(authContent);
            LogInUser(obj?.Access_Token);
            return obj?.Access_Token;
        }

        public void LogInUser(string? tokenBearer)
        {
            ClearApiDefaultRequestHeaders();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenBearer}");
        }

        public async Task<string?> RequestSecurityStamp(string? password)
        {
            var data = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("password", password?? "")
            });

            var url = _config[key: "URL:apiUrl"] + _config[key: "URL:request-security-stamp"];
            using HttpResponseMessage response = await _apiClient.PostAsync(url, data);

            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadAsStringAsync();
            var obj = JsonConvert.DeserializeObject<ResultDTO<string>>(result);

            return obj?.Data;
        }

        public void LogOffUser()
        {
            ClearApiDefaultRequestHeaders();
        }

        private void ClearApiDefaultRequestHeaders()
        {
            _apiClient.DefaultRequestHeaders.Clear();
            _apiClient.DefaultRequestHeaders.Accept.Clear();
        }
        public void Dispose()
        {
            ClearApiDefaultRequestHeaders();
        }
    }


}
