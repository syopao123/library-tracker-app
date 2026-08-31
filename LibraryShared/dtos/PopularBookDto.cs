using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryShared.dtos
{
    public class PopularBookDto
    {
        public required string OpenLibraryKey { get; set; }
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public List<string>? AuthorNames { get; set; }
        public string? ImageUrl { get; set; }
        public int OwnerCount { get; set; }
        public bool IsOwnedByUser { get; set; }

    }
}