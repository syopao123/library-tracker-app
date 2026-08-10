using System.ComponentModel.DataAnnotations;

namespace LibraryTrackerApp.Data
{
    public class Book : IValidatableObject
    {
        [Required(ErrorMessage = "Book title is required")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Book author is required")]
        public required string Author { get; set; }

        public string? Genre { get; set; }
        
        public ReadingStatus Status { get; set; } = ReadingStatus.ToRead;

        [Range(1, 5)]
        public int? Rating { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public string PersonalNotes { get; set; } = "";

        public string ImageUrl { get; set; } = "images/book.jpg";

        // Checks if user has rated the book once finished reading
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Status == ReadingStatus.Finished && Rating == null)
            {
                yield return new ValidationResult("Rating is required once a book is marked as Finished.", [ nameof(Rating) ] );
            }
        }
    }

    public enum ReadingStatus
    {
        ToRead, Reading, Finished
    }
}