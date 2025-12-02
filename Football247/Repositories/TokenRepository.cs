using Football247.Authorization;
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

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // NEW
            var permissions = await (
                    from role in _authDbContext.Roles
                    join claim in _authDbContext.RoleClaims on role.Id equals claim.RoleId
                    where roles.Contains(role.Name!) &&
                        claim.ClaimType == CustomClaimTypes.Permission
                    select claim.ClaimValue)
                .Distinct()
                .ToArrayAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            foreach (var permission in permissions)
            {
                claims.Add(new Claim(CustomClaimTypes.Permission, permission));
            }

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

            return new LoginResponseDto
            {
                UserId = user.Id,
                FullName = user.UserName,
                JwtToken = accessTokenString,
                RefreshToken = refreshToken.Token
            };
        }
        

        public async Task<LoginResponseDto> RefreshTokensAsync(string refreshToken)
        {
            var storedToken = await _authDbContext.RefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            // 2. Xác thực token (không cần accessToken cũ nữa)
            if (storedToken == null || storedToken.ExpiryDate <= DateTime.UtcNow || storedToken.IsUsed)
            {
                throw new SecurityTokenException("Invalid, expired, or used Refresh Token");
            }

            // Lấy UserId trực tiếp từ storedToken
            var userId = storedToken.UserId;
            if (string.IsNullOrEmpty(userId))
            {
                throw new SecurityTokenException("User ID not found in refresh token");
            }

            // 3. Đánh dấu token cũ là đã sử dụng (Rotation)
            storedToken.IsUsed = true;
            await _authDbContext.SaveChangesAsync();

            // 4. Tạo cặp token mới
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new SecurityTokenException("User not found");
            }

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
