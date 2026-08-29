using System.Net.Http.Json;
using Microsoft.JSInterop;
using System.Net.Http.Headers;
using LibraryShared.dtos;
using System.Text.Json;
using LibraryShared.classes;
using LibraryShared.enums;
using System.Net;

namespace LibraryTrackerApp.Services
{
    public class BookService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILocalStorageService _localStorage;

        private string? AccessToken => _localStorage.GetItem<string>("accessToken") ?? null;

        public BookService(IHttpClientFactory httpClientFactory, ILocalStorageService localStorage)
        {
            _httpClientFactory = httpClientFactory;
            _localStorage = localStorage;
        }

        public async Task<OpenLibrarySearchResponse?> SearchBookAsync(BookSearchDto dto)
        {
            if (string.IsNullOrEmpty(AccessToken))
                return null;

            var httpClient = _httpClientFactory.CreateClient("WebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            using var httpResponseMessage = await httpClient.PostAsJsonAsync("books/search", dto);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<OpenLibrarySearchResponse>(contentStream);
            }
            return null;
        }

        public async Task<List<BookDto>?> GetUserBooksAsync()
        {
            if (string.IsNullOrEmpty(AccessToken))
                return null;

            var httpClient = _httpClientFactory.CreateClient("WebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            var httpResponseMessage = await httpClient.GetAsync("books/library");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                var books = await JsonSerializer.DeserializeAsync<List<BookDto>>(contentStream);
                return books;
            }

            return new();
        }

        public async Task<(bool, string)> AddBookAsync(AddBookDto dto)
        {
            if (string.IsNullOrEmpty(AccessToken))
                return (false, "Invalid user authentication.");

            var httpClient = _httpClientFactory.CreateClient("WebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            using var httpResponseMessage = await httpClient.PostAsJsonAsync("books", dto);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                var content = await JsonSerializer.DeserializeAsync<BookDto>(contentStream);
                
                if (content != null && content.Title is not null)
                {
                    return (true, $"{content.Title} by {content.CustomAuthor} has been successfully added to the library.");
                }
            }
            var message = await httpResponseMessage.Content.ReadAsStringAsync();
            return (false, message);
        }

        public async Task<(bool, string)> UpdateBookAsync(UpdateBookDto dto)
        {
            if (string.IsNullOrEmpty(AccessToken))
                return (false, "Invalid user authentication");
            
            var httpClient = _httpClientFactory.CreateClient("WebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            using var httpResponseMessage = await httpClient.PatchAsJsonAsync("books", dto);

            if (httpResponseMessage.IsSuccessStatusCode)
                return (true, "You successfully updated your book.");
                
            var message = await httpResponseMessage.Content.ReadAsStringAsync();
            return (false, message);
        }

        public async Task<(bool, string)> RemoveBookFromLibraryAsync(BookDto dto)
        {
            if (string.IsNullOrEmpty(AccessToken))
                return (false, "Invalid authentication");

            var httpClient = _httpClientFactory.CreateClient("WebApi");
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", AccessToken);
            using var httpResponseMessage = await httpClient.DeleteAsync($"books/{dto.Id}");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                return (true, "Success");
            }

            var message = await httpResponseMessage.Content.ReadAsStringAsync();
            return (false, message);
        }
    }
}