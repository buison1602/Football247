using Microsoft.AspNetCore.Identity;

namespace Football247.Models.Entities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string? AvatarUrl { get; set; }
        public int Points { get; set; }
        public int SpinCount { get; set; }
        public bool ReceiveInAppNotifications { get; set; } = true;
        public bool ReceiveEmailNotifications { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
