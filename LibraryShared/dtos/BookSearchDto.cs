using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryShared.dtos
{
    public class BookSearchDto
    {
        [Required(ErrorMessage = "Book title is required.")]
        public required string Title { get; set; }
        [Required(ErrorMessage = "Book author name is required.")]
        public required string Author { get; set; }
    }
}