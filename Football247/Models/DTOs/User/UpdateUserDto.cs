namespace Football247.Models.DTOs.User
{
    public class UpdateUserDto
    {
        public string? AvatarUrl { get; set; }
        public int Points { get; set; }
        public int SpinCount { get; set; }
    }
}
