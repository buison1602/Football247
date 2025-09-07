using Football247.Models.DTOs.Auth;
using Football247.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Football247.Repositories.IRepository
{
    public interface ITokenRepository
    {
        Task<LoginResponseDto> CreateTokensAsync(ApplicationUser user);
        Task<LoginResponseDto> RefreshTokensAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshTokenString);
    }
}
