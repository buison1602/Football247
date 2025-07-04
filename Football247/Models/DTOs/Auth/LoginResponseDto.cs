namespace Football247.Models.DTOs.Auth
{
    public class LoginResponseDto
    {
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
