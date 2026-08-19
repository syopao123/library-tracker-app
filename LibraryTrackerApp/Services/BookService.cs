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

        public async Task<OpenLibrarySearchResponse?> SearchBookAsync(BookSearchDto dto)
        {
            try
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
            catch (Exception ex)
            {
                var response = new OpenLibrarySearchResponse()
                {
                    IsSuccess = false,
                    ErrorMessage = ex.Message
                };
                return response;
            }
        }

        public async Task<List<BookDto>?> GetUserBooksAsync()
        {
            string? accessToken = GetAccessToken();

            if (accessToken is not null)
            {
                string? userEmail = _localStorage.GetItem<string>("userEmail");

                if (string.IsNullOrWhiteSpace(userEmail))
                    return null;
                
                var userDto = new UserDto() { Email = userEmail };

                var httpClient = _httpClientFactory.CreateClient("WebApi");
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                using var httpResponseMessage = await httpClient.PostAsJsonAsync(httpClient.BaseAddress + "/books/library", userDto);

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                    var books = await JsonSerializer.DeserializeAsync<List<BookDto>>(contentStream);
                    return books;
                }
            }
            return null;
        }

        // TODO: Remove user email property from AddBookDto, retrieve it using Claims instead.
        public async Task<BookDto?> AddBookAsync(AddBookDto dto)
        {
            try
            {
                string? accessToken = GetAccessToken();

                if (accessToken is not null)
                {
                    dto.UserEmail = _localStorage.GetItem<string>("userEmail");
                    var httpClient = _httpClientFactory.CreateClient("WebApi");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using var httpResponseMessage = await httpClient.PostAsJsonAsync(httpClient.BaseAddress + "/books", dto);

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                        return await JsonSerializer.DeserializeAsync<BookDto>(contentStream);
                    }
                }
                // TODO: Make it so that the user gets a new accessToken again
                // Also handle when api sends back unauthorized
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task RemoveBookFromLibraryAsync(BookDto dto)
        {
            try
            {
                string? accessToken = GetAccessToken();
                string? userEmail = _localStorage.GetItem<string>("userEmail");

                if (accessToken is not null && userEmail is not null)
                {
                    var httpClient = _httpClientFactory.CreateClient("WebApi");
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                    using var httpResponseMessage = await httpClient.DeleteAsync(httpClient.BaseAddress + $"/books/{dto.Id}");

                    if (httpResponseMessage.IsSuccessStatusCode)
                    {
                        // TODO: Implement proper success delete msg to user
                        // var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();
                        // return await JsonSerializer.DeserializeAsync<BookDto>(contentStream);
                    }
                }
                // TODO: Make it so that the user gets a new accessToken again
                // Also handle when api sends back unauthorized
            }
            catch (Exception)
            {
            }
        }
    }
}