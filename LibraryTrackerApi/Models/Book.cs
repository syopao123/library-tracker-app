using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using LibraryShared.enums;

namespace LibraryTrackerApi.Models
{
    public class Book
    {
        public Guid Id { get; set; }

        public string OpenLibraryKey { get; set; } = null!;

        [Required(ErrorMessage = "Book title is required")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "Book author is required")]
        public required List<string> AuthorNames { get; set; }

        public List<string>? Subjects { get; set; }
        public string ImageUrl { get; set; } = "images/book.jpg";

        public List<BookOwner> BookOwners { get; set; } = new();
    }
}