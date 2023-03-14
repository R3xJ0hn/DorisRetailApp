using Blazored.LocalStorage;
using DorisApp.Data.Library.API;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace DorisApp.WebPortal.Authentication
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _config;
        private readonly IAPIHelper _apiHelpder;
        private readonly AuthenticationState _anonymous;

        public AuthStateProvider(
                HttpClient httpClient,
                ILocalStorageService localStorage,
                IConfiguration config,
                IAPIHelper apiHelpder)
        {
            _httpClient = httpClient;
            _localStorage = localStorage;
            _config = config;
            _apiHelpder = apiHelpder;
            _anonymous = new AuthenticationState(user: new ClaimsPrincipal(identity: new ClaimsIdentity()));
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(key: _config[key: "authTokenStorageKey"]);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "bearer", token);
            return new AuthenticationState(
                user: new ClaimsPrincipal(
                    identity: new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token),
                    authenticationType: "jwtAuthType")));
        }

        public async Task NotifyUserAuthentication(string token)
        {
            Task<AuthenticationState> authState;

            try
            {
                _apiHelpder.LogInUser(token); 
                var authenticatedUser = new ClaimsPrincipal(
                        identity: new ClaimsIdentity(JwtParser.ParseClaimsFromJwt(token),
                        authenticationType: "jwtAuthType"));
                authState = Task.FromResult(new AuthenticationState(authenticatedUser));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                string key = _config[key: "authTokenStorageKey"];
                await _localStorage.RemoveItemAsync(key);
                authState = Task.FromResult(_anonymous);
            }

            NotifyAuthenticationStateChanged(authState);
        }

        public void NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            _apiHelpder.LogOffUser();
            NotifyAuthenticationStateChanged(authState);
        }

    }

}
