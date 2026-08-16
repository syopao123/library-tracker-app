
using System.Text.Json;
using LibraryShared.classes;
using LibraryShared.dtos;
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
        private async Task<OpenLibrarySearchResponse?> ApiSearchBook(string title, string author, int? publishYear)
        {
            title = title.Replace(' ', '+');
            author = author.Replace(' ', '+');

            string query = publishYear is null ? $"title={title}&author={author}&fields=title,author_name,cover_i,publish_year,subject" 
                    : $"title={title}&author={author}&publish_year={publishYear}&fields=title,author_name,cover_i,publish_year,subject";

            var httpClient = _httpClientFactory.CreateClient("BookSearchApi");
            using var httpResponseMessage = await httpClient.GetAsync(httpClient.BaseAddress + query);

            if (httpResponseMessage.IsSuccessStatusCode)
            {
                using var contentStream = await httpResponseMessage.Content.ReadAsStreamAsync();

                return await JsonSerializer.DeserializeAsync<OpenLibrarySearchResponse>(contentStream);
            }
            return null;
        }

        public async Task<OpenLibrarySearchResponse?> SearchBookAsync(BookSearchDto dto)
        {
            return await ApiSearchBook(dto.Title, dto.Author, dto.PublishYear);
        }
    }
}