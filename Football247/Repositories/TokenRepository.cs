using Football247.Data;
using Football247.Models.DTOs.Auth;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Football247.Repositories
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration;
        private readonly Football247DbContext _authDbContext;
        private readonly UserManager<ApplicationUser> _userManager;

        public TokenRepository(IConfiguration configuration, Football247DbContext authDbContext, UserManager<ApplicationUser> userManager)
        {
            _configuration = configuration;
            _authDbContext = authDbContext;
            _userManager = userManager;
        }

        public async Task<LoginResponseDto> CreateTokensAsync(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                // Sử dụng user.Id từ ApplicationUser
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var accessTokenExpiry = DateTime.UtcNow.AddMinutes(_configuration.GetValue<int>("Jwt:AccessTokenValidityInMinutes"));

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: accessTokenExpiry,
                claims: claims,
                signingCredentials: credentials
            );

            var accessTokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Tạo và lưu Refresh Token vào DB
            var refreshToken = new RefreshToken()
            {
                IsUsed = false,
                UserId = user.Id,
                ExpiryDate = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("Jwt:RefreshTokenValidityInDays")),
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
            };

            await _authDbContext.RefreshTokens.AddAsync(refreshToken);
            await _authDbContext.SaveChangesAsync();

            // Trả về DTO chứa cả hai token
            return new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.UserName,
                JwtToken = accessTokenString,
                RefreshToken = refreshToken.Token
            };
        }
        

        public async Task<LoginResponseDto> RefreshTokensAsync(TokenRequestDto tokenRequestDto)
        {
            var principal = DecodeJwtToken(tokenRequestDto.AccessToken);
            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                throw new SecurityTokenException("Invalid token claims");
            }

            var storedToken = await _authDbContext.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestDto.RefreshToken);

            if (storedToken == null || storedToken.UserId != userId || storedToken.ExpiryDate <= DateTime.UtcNow || storedToken.IsUsed)
            {
                throw new SecurityTokenException("Invalid Refresh Token");
            }

            storedToken.IsUsed = true;
            await _authDbContext.SaveChangesAsync();

            var user = await _userManager.FindByIdAsync(userId);
            return await CreateTokensAsync(user);
        }


        public async Task<bool> LogoutAsync(string refreshTokenString)
        {
            var token = await _authDbContext.RefreshTokens
                .FirstOrDefaultAsync(t => t.Token == refreshTokenString);

            if (token == null || token.IsUsed || token.ExpiryDate <= DateTime.UtcNow)
            {
                return true;
            }

            // Vô hiệu hóa token cụ thể này
            token.IsUsed = true;

            var changedCount = await _authDbContext.SaveChangesAsync();
            return changedCount > 0;
        }


        // validate access token (JWT token) without checking expiration
        private ClaimsPrincipal DecodeJwtToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                throw new SecurityTokenException("Invalid token");
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = true,
                ValidAudience = _configuration["Jwt:Audience"],
                ValidateLifetime = false // Quan trọng: không kiểm tra thời hạn
            };

            // principal includes user claims and roles
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
            if (validatedToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token algorithm");
            }

            return principal;
        }
    }
}
