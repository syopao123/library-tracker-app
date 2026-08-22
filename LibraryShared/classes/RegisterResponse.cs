using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryShared.classes
{
    public class RegisterResponse
    {
        public Dictionary<string, string[]> Errors { get; set; } = new();
    }
}