using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace LibraryTrackerApi.Models
{
    public class User : IdentityUser
    {
        public List<BookOwner> BookOwners { get; set; } = new();
    }
}