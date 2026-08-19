using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using LibraryShared.enums;

namespace LibraryShared.dtos
{
    public class BookDto
    {
        [JsonPropertyName("bookId")]
        public Guid Id { get; set; }
        
        [Required]
        [JsonPropertyName("title")]
        public required string Title { get; set; }

        [Required]
        [JsonPropertyName("author")]
        public required string Author { get; set; }

        [JsonPropertyName("coverI")]
        public int? CoverI { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("genre")]
        public string? Genre { get; set; }

        [JsonPropertyName("status")]
        public ReadingStatus Status { get; set; }

        [JsonPropertyName("rating")]
        public int? Rating { get; set; }

        [JsonPropertyName("personalNotes")]
        public string? PersonalNotes { get; set; }

        [JsonPropertyName("firstPublishYear")]
        public int? FirstPublishYear { get; set; }

        [JsonPropertyName("publishYears")]
        public int[]? PublishYears { get; set; }

        [JsonPropertyName("authorName")]
        public List<string>? AuthorName { get; set; }

        [JsonPropertyName("subjects")]
        public List<string>? Subjects { get; set; }
    }
}