using System.Net.Http.Json;
using LibraryShared.classes;
using LibraryShared.dtos;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Security.Claims;

namespace LibraryTrackerApp.Services
{
    public class AuthService : AuthenticationStateProvider
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorage;

        public string? AccessToken => _localStorage.GetItem<string>("accessToken");

        public AuthService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        public string? GetValidAccessTokenAsync()
        {
            return _localStorage.GetItem<string>("accessToken");
        }

        // Refreshes the user's access token
        public async Task<bool> RefreshTokenUserAsync()
        {
            string? token = _localStorage.GetItem<string>("refreshToken");

            if (string.IsNullOrEmpty(token) == false)
            {
                var httpClient = _httpClientFactory.CreateClient("WebApi");
                var httpResponse = await httpClient.PostAsJsonAsync("refresh", new { refreshToken = token});

                if (httpResponse.IsSuccessStatusCode)
                {
                    var contentStream = await httpResponse.Content.ReadFromJsonAsync<LoginResponse>();

                    if (contentStream is not null)
                    {
                        _localStorage.SetItem<string>("accessToken", contentStream.AccessToken);
                        _localStorage.SetItem<string>("refreshToken", contentStream.RefreshToken);

                        return true;
                    }
                }
            }
            return false;
        }

        // Checks if current user is logged in or not
        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            string? accessToken;

            try
            {
                accessToken = _localStorage.GetItem<string>("accessToken");
            } catch
            {
                accessToken = null;
            }

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                var anonymousUser = new ClaimsPrincipal(new ClaimsIdentity());
                return Task.FromResult(new AuthenticationState(anonymousUser));
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Email, _localStorage.GetItem<string>("userEmail") ?? string.Empty)
            };

            var user = new ClaimsIdentity(claims, authenticationType: "Bearer");
            var authenticatedUser = new ClaimsPrincipal(user);

            return Task.FromResult(new AuthenticationState(authenticatedUser));
        }

        public async Task<LoginResponse?> LoginUserAsync(LoginDto dto)
        {
            try
            {
                var httpClient = _httpClientFactory.CreateClient("WebApi");
                var loginResponse = await httpClient.PostAsJsonAsync("login", dto);

                if (loginResponse.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var errorLoginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();
                    return errorLoginData;
                }

                if (loginResponse.StatusCode == HttpStatusCode.OK)
                {
                    var successLoginData = await loginResponse.Content.ReadFromJsonAsync<LoginResponse>();

                    if (successLoginData is null)
                        return null;

                    _localStorage.SetItem("userEmail", dto.Email);
                    _localStorage.SetItem("accessToken", successLoginData.AccessToken);
                    _localStorage.SetItem("refreshToken", successLoginData.RefreshToken);

                    NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                    return successLoginData;
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<RegisterResponse?> RegisterUserAsync(RegisterDto dto)
        {
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            var registerResponse = await httpClient.PostAsJsonAsync("register", dto);
            
            if (registerResponse.IsSuccessStatusCode == false)
                return await registerResponse.Content.ReadFromJsonAsync<RegisterResponse>();
            else
                return null;
        }

        public void Logout()
        {
            _localStorage.RemoveItem("accessToken");
            _localStorage.RemoveItem("refreshToken");
            _localStorage.RemoveItem("userEmail");
            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
        }
    }
}