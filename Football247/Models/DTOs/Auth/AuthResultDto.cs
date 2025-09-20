namespace Football247.Models.DTOs.Auth
{
    public class AuthResultDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
        public bool Succeeded { get; set; } = true;
        public IEnumerable<string> Errors { get; set; }
    }
}
