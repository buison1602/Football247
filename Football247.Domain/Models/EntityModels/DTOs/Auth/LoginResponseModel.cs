namespace Football247.Domain.Models.EntityModels.DTOs.Auth
{
    public class LoginResponseModel
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string JwtToken { get; set; }
        public string RefreshToken { get; set; }
    }
}
