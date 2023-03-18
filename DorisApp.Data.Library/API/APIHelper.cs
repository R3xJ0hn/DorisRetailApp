using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Net.Http.Headers;

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

        public void LogInUser(string tokenBearer)
        {
            ClearApiDefaultRequestHeaders();
            _apiClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            _apiClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {tokenBearer}");
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
