using Football247.Models.DTOs.Auth;

namespace Football247.Services.IService
{
    public interface IAuthService
    {
        Task<AuthResultDto> RegisterAsync(RegisterRequestDto registerRequestDto);
        Task<AuthResultDto> LoginAsync(LoginRequestDto loginRequestDto);
        Task<AuthResultDto> RefreshTokenAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshToken);
        Task<AuthResultDto> GoogleLoginAsync(GoogleLoginRequestDto request);
    }
}
