using System.ComponentModel.DataAnnotations;
using LibraryShared.enums;

namespace LibraryShared.dtos
{
    public class UpdateBookDto : IValidatableObject
    {
        public required Guid Id { get; set; }
        public string? CustomTitle { get; set; }
        public string? CustomAuthor { get; set; }
        public string? CustomImageUrl { get; set; }
        public string? CustomGenre { get; set; }
        public required ReadingStatus UpdatedStatus { get; set; }
        public int? UpdatedRating { get; set; }
        public string? UpdatedPersonalNotes { get; set; }

        // Checks if user has rated the book once finished reading
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UpdatedStatus == ReadingStatus.Finished && UpdatedRating == null)
            {
                yield return new ValidationResult("Rating is required once a book is marked as Finished.", [ nameof(UpdatedRating) ] );
            }
        }

    }
}