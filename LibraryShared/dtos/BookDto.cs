using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LibraryShared.enums;

namespace LibraryShared.dtos
{
    public class BookDto
    {
        public required string Title { get; set; }
        public required string Author { get; set; }
        public string? ImageUrl { get; set; }
        public string? Genre { get; set; }
        public ReadingStatus Status { get; set; }
        public int? Rating { get; set; }
        public string? PersonalNotes { get; set; }
    }
}