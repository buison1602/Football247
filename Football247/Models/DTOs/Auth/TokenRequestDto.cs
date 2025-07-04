namespace Football247.Models.DTOs.Auth
{
    public class TokenRequestDto
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
