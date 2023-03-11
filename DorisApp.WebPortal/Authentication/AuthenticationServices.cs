﻿using Blazored.LocalStorage;
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
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _config;
        private readonly string _authTokenStorageKey = string.Empty;

        public AuthenticationServices(HttpClient client,
            AuthenticationStateProvider authStateProvider,
            ILocalStorageService localStorage,
            IConfiguration config)
        {
            _client = client;
            _authStateProvider = authStateProvider;
            _localStorage = localStorage;
            _config = config;
            _authTokenStorageKey = _config[key: "authTokenStorageKey"];
        }

        public async Task<AuthenticatedUserModel> Login(AuthenticationUserModel userAuth)
        {
            var data = new FormUrlEncodedContent(new[]
{
                new KeyValuePair<string, string>("email", userAuth.Email),
                new KeyValuePair<string, string>("password", userAuth.Password)
            });

            var api = _config[key: "apiUrl"] + _config[key: "apiLogin"];
            var authResult = await _client.PostAsync(requestUri: api, data);
            var authContent = await authResult.Content.ReadAsStringAsync();

            if (authResult.IsSuccessStatusCode == false)
            {
                return null;
            }

            var result = JsonSerializer.Deserialize<AuthenticatedUserModel>(
                authContent, options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true });


            await _localStorage.SetItemAsync(key: _authTokenStorageKey, result.Access_Token);

            ((AuthStateProvider)_authStateProvider).NotifyUserAuthentication(result.Access_Token);

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "bearer", result.Access_Token);

            return result;
        }

        public async Task Logout()
        {
            await _localStorage.RemoveItemAsync(key: _authTokenStorageKey);

            ((AuthStateProvider)_authStateProvider).NotifyUserLogout();
            _client.DefaultRequestHeaders.Authorization = null;
        }

    }

}