using System.Net.Http.Headers;
using System.Net.Http.Json;
using LibraryShared.dtos;
using Microsoft.JSInterop;

namespace LibraryTrackerApp.Services
{
    public class BrowserService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorage;

        private string? AccessToken => _localStorage.GetItem<string>("accessToken");

        public BrowserService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        public async Task<List<CommunityBookDto>?> GetCommunityBooksAsync()
        {
            if (string.IsNullOrEmpty(AccessToken)) return new();

            var httpClient = _httpClientFactory.CreateClient("WebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var communityBooks = await httpClient.GetFromJsonAsync<List<CommunityBookDto>>("books/community");

            return communityBooks;
        }

        public async Task<List<PopularBookDto>?> GetPopularBooksAsync(int count)
        {
            if (string.IsNullOrEmpty(AccessToken)) return new();

            var httpClient = _httpClientFactory.CreateClient("WebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var popularBooks = await httpClient.GetFromJsonAsync<List<PopularBookDto>>($"books/popular-books/{count}");

            return popularBooks;
        }

    }
}