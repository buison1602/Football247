namespace Football247.Models.DTOs.User
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string AvatarUrl { get; set; }
        public int Points { get; set; }
        public int SpinCount { get; set; }
    }
}
