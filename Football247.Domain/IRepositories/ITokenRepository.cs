using Football247.Domain.Models.EntityModels.DTOs.Auth;
using Football247.Models.Entities;
using Microsoft.AspNetCore.Identity;

namespace Football247.Repositories.IRepository
{
    public interface ITokenRepository
    {
        Task<LoginResponseModel> CreateTokensAsync(ApplicationUser user);
        Task<LoginResponseModel> RefreshTokensAsync(string refreshToken);
        Task<bool> LogoutAsync(string refreshTokenString);
    }
}
