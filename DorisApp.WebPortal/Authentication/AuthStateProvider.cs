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
        private readonly string _autLocalKey;

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
            _autLocalKey = _config[key: "authTokenStorageKey"];

        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var token = await _localStorage.GetItemAsync<string>(key: _autLocalKey);

            if (string.IsNullOrWhiteSpace(token))
            {
                return _anonymous;
            }

            _apiHelpder.LogInUser(token);
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
                await _localStorage.SetItemAsync(key: _autLocalKey, token);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await _localStorage.RemoveItemAsync(_autLocalKey);
                _apiHelpder.LogOffUser();
                authState = Task.FromResult(_anonymous);
            }

            NotifyAuthenticationStateChanged(authState);
        }

        public async Task NotifyUserLogout()
        {
            var authState = Task.FromResult(_anonymous);
            _apiHelpder.LogOffUser();
            NotifyAuthenticationStateChanged(authState);

            await _localStorage.RemoveItemAsync(key: _autLocalKey);
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

    }

}
