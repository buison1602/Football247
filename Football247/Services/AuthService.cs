using Football247.Models.DTOs.Auth;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.IService;
using Microsoft.AspNetCore.Identity;

namespace Football247.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        public AuthService(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        public async Task<AuthResultDto> RegisterAsync(RegisterRequestDto registerRequestDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(registerRequestDto.Email);

            Console.WriteLine("/n/n---------------" + existingUser + "---------------------\n\n");

            if (existingUser != null)
            {
                throw new InvalidOperationException("Email is exist");
            }

            var identityUser = new ApplicationUser
            {
                Email = registerRequestDto.Email,
                UserName = registerRequestDto.Email
            };

            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRoleAsync(identityUser, "User");

                if (identityResult.Succeeded)
                {
                    var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(identityUser);

                    return new AuthResultDto
                    {
                        UserId = tokens.UserId,
                        FullName = tokens.FullName,
                        JwtToken = tokens.JwtToken,
                        RefreshToken = tokens.RefreshToken
                    };
                }
            }

            return new AuthResultDto
            {
                Succeeded = false,
                Errors = identityResult.Errors.Select(e => e.Description)
            };
        }


        public async Task<AuthResultDto> LoginAsync(LoginRequestDto loginRequestDto)
        {
            var identityUser = await _userManager.FindByEmailAsync(loginRequestDto.Email);
            if (identityUser == null)
            {
                return null; 
            }

            var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password);
            if (!checkPasswordResult)
            {
                return null;
            }

            var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(identityUser);
            return new AuthResultDto
            {
                UserId = tokens.UserId,
                FullName = tokens.FullName,
                JwtToken = tokens.JwtToken,
                RefreshToken = tokens.RefreshToken
            };
        }


        public async Task<AuthResultDto> RefreshTokenAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                throw new ArgumentNullException(nameof(refreshToken), "Refresh token cannot be null or empty.");
            }

            // Logic trong TokenRepository của bạn sẽ xử lý việc ném SecurityTokenException
            var result = await _unitOfWork.TokenRepository.RefreshTokensAsync(refreshToken);

            return new AuthResultDto
            {
                UserId = result.UserId,
                FullName = result.FullName,
                JwtToken = result.JwtToken,
                RefreshToken = result.RefreshToken
            };
        }


        public async Task<bool> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return false;
            }
            return await _unitOfWork.TokenRepository.LogoutAsync(refreshToken);
        }


        public Task<AuthResultDto> GoogleLoginAsync(GoogleLoginRequestDto request)
        {
            throw new NotImplementedException();
        }
    }
}
