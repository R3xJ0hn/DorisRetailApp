using Blazored.LocalStorage;
using DorisApp.WebPortal.Model;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DorisApp.WebPortal.Authentication
{
    public class AuthenticationServices : IAuthenticationServices
    {
        private readonly HttpClient _client;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly IConfiguration _config;

        public AuthenticationServices(HttpClient client,
            AuthenticationStateProvider authStateProvider,
            ILocalStorageService localStorage,
            IConfiguration config)
        {
            _client = client;
            _authStateProvider = authStateProvider;
            _config = config;
        }

        public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userAuth)
        {
            var data = new FormUrlEncodedContent(new[]
{
                new KeyValuePair<string, string>("email", userAuth.Email),
                new KeyValuePair<string, string>("password", userAuth.Password)
            });

            var apiURL = _config[key: "URL:apiUrl"] + _config[key: "URL:login"];
            var authResult = await _client.PostAsync(requestUri: apiURL, data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (authResult.IsSuccessStatusCode == false)
            {
                return null;
            }

            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(
                authContent, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Access_Token);

            return result;
        }

        public async Task Logout()
        {
            await ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
        }

    }

}
