using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace LibraryShared.dtos
{
    public class CommunityBookDto
    {
        
        public Guid BookId { get; set; }
        public required string Username { get; set; }
        public required string Title { get; set; }
        public required DateTime DateAdded { get; set; }

    }
}