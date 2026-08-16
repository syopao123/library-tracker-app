
using System.ComponentModel.DataAnnotations;

namespace LibraryShared.dtos
{
    public class BookSearchDto
    {
        [Required(ErrorMessage = "Book title is required.")]
        public required string Title { get; set; }
        [Required(ErrorMessage = "Book author name is required.")]
        public required string Author { get; set; }
        [Range(1200, 2026, ErrorMessage = "Please enter a year between 1200 to 2026.")]
        public int? PublishYear { get; set; }
    }
}