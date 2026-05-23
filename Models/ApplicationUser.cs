using Microsoft.AspNetCore.Identity;

namespace WatchReadLibrary.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? AvatarPath { get; set; }
    }
}