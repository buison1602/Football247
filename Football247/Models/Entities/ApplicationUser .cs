using Microsoft.AspNetCore.Identity;
using System.Data.Common;

namespace Football247.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatarUrl { get; set; }
        public int Points { get; set; }
        public int SpinCount { get; set; }
    }
}
