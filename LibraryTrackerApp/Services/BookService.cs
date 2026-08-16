using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using LibraryShared.dtos;
using System.Text.Json;
using LibraryShared.classes;

namespace LibraryTrackerApp.Services
{
    public class BookService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorage;

        public BookService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        public async Task<OpenLibrarySearchResponse?> SearchBookAsync(BookSearchDto dto)
        {
            string? accessToken = GetAccessToken();

            if (accessToken is not null)
            {
                var httpClient = _httpClientFactory.CreateClient("WebApi");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                using var httpResponseMessage = await httpClient.PostAsJsonAsync(httpClient.BaseAddress + "/books/search", dto);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    return await JsonSerializer.DeserializeAsync<OpenLibrarySearchResponse>(contentStream);
                }
            }
            // TODO: Make it so that the user gets a new accessToken again
            // Also handle when api sends back unauthorized
            return null;
        }

        public async Task<List<BookDto>?> GetUserBooksAsync()
        {
            string? accessToken = GetAccessToken();

            if (accessToken is not null)
            {
                var httpClient = _httpClientFactory.CreateClient("WebApi");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                using var httpResponseMessage = await httpClient.GetAsync("books");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    var books = await JsonSerializer.DeserializeAsync<List<BookDto>>(contentStream);
                    return books;
                }
            }

            return null;
        }

        private string? GetAccessToken()
        {
            string? accessToken;
            try
            {
                accessToken = _localStorage.GetItem<string>("accessToken");
            }
            catch
            {
                accessToken = null;
            }
            return accessToken;
        }
    }
}