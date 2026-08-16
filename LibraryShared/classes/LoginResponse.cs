using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryShared.classes
{
    public class LoginResponse
    {
        public required string TokenType {get; set; }
        public required string AccessToken { get; set; }
        public required int ExpiresIn { get; set; }
        public required string RefreshToken { get; set; }

        public string? Title { get; set; }
        public int? Status { get; set; }
        public string? Detail { get; set; }
    }
}