using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using LibraryShared.enums;

namespace LibraryShared.dtos
{
    public class AddBookDto : IValidatableObject
    {
        [Required]
        public required string Title { get; set; }
        
        [Required]
        public required string OpenLibraryKey { get; set; }

        [Required]
        public required List<string> AuthorNames { get; set; }
        public List<string>? Subjects { get; set; }

        public int? CoverI { get; set; }

        public string? CustomGenre { get; set; }
        
        public ReadingStatus Status { get; set; } = ReadingStatus.ToRead;

        public int? Rating { get; set; }

        public string? PersonalNotes { get; set; }

        [Required(ErrorMessage = "An author is required.")]
        public required string CustomAuthor { get; set; }

        // Checks if user has rated the book once finished reading
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Status == ReadingStatus.Finished && Rating == null)
            {
                yield return new ValidationResult("Rating is required once a book is marked as Finished.", [ nameof(Rating) ] );
            }
        }
    }
}