using System.ComponentModel.DataAnnotations;
using LibraryShared.enums;

namespace LibraryTrackerApi.Models
{
    public class BookOwner : IValidatableObject
    {
        public required string UserId { get; set; }
        public Guid BookId { get; set; }

        public ReadingStatus Status { get; set; } = ReadingStatus.ToRead;

        [Range(1, 5)]
        public int? Rating { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        public string? PersonalNotes { get; set; }

        // Checks if user has rated the book once finished reading
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Status == ReadingStatus.Finished && Rating == null)
            {
                yield return new ValidationResult("Rating is required once a book is marked as Finished.", [ nameof(Rating) ] );
            }
        }

        public required User User { get; set; }
        public required Book Book { get; set; }

    }
}