using System.ComponentModel.DataAnnotations;
using LibraryShared.enums;

namespace LibraryShared.dtos
{
    public class BookDto
    {
        [Required]
        public required string Title { get; set; }
        [Required]
        public required string Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? Genre { get; set; }
        public ReadingStatus Status { get; set; }
        public int? Rating { get; set; }
        public string? PersonalNotes { get; set; }
    }
}