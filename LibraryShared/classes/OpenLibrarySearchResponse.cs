using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibraryShared.classes
{
    public class OpenLibrarySearchResponse
    {
        [JsonPropertyName("numFound")]
        public int NumFound { get; set; }
        [JsonPropertyName("docs")]
        public List<OpenLibraryDoc> Docs { get; set; } = new();
        public bool IsSuccess { get; set; } = true;
        public string? ErrorMessage { get; set; }
    }

    public class OpenLibraryDoc
    {
        [JsonPropertyName("title")]
        public string? Title { get; set; }
        [JsonPropertyName("author_name")]
        public List<string>? AuthorName { get; set; }
        [JsonPropertyName("cover_i")]
        public int? CoverI { get; set; }
        [JsonPropertyName("first_publish_year")]
        public int? FirstPublishYear { get; set; }
        [JsonPropertyName("publish_year")]
        public int[]? PublishYears { get; set; }
        [JsonPropertyName("subject")]
        public List<string>? Subjects { get; set; }
    }
}