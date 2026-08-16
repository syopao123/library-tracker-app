
using System.Text.Json;
using LibraryShared.classes;
using Microsoft.Net.Http.Headers;

namespace LibraryTrackerApi.Services
{
    public class OpenLibraryService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private OpenLibrarySearchResponse? SearchedBookResults { get; set; }
        
        public OpenLibraryService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;            
        }

        // Makes an API request using Open Library's Book Search API
        private async Task SearchBookTitle(string title, string author)
        {
            title = title.Replace(' ', '+');
            author = author.Replace(' ', '+');

            var httpClient = _httpClientFactory.CreateClient("BookSearchApi");
            using var httpResponseMessage = await httpClient.GetAsync(httpClient.BaseAddress + $"title={title}&author={author}&fields=title,author_name,cover_i");

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                SearchedBookResults = await JsonSerializer.DeserializeAsync<OpenLibrarySearchResponse>(contentStream);
            }
        }

        public async Task<OpenLibrarySearchResponse>? SearchBookAsync(string title, string author)
        {
            await SearchBookTitle(title, author);
            return SearchedBookResults!;
        }
    }
}