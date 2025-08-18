using Football247.Data;
using Football247.Models.DTOs.Auth;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Google.Apis.Auth;
using Microsoft.Extensions.Configuration;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Football247DbContext _appDbContext;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, 
            IUnitOfWork unitOfWork,
            Football247DbContext appDbContext,
            IConfiguration configuration)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
            this._appDbContext = appDbContext;
            this._configuration = configuration;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
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
                    return Ok("User registered successfully. Please login.");
                }
            }

            var errorMessages = identityResult.Errors.Select(e => e.Description);
            return BadRequest($"User registration failed: {string.Join(", ", errorMessages)}");
        }

        
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var identityUser = await _userManager.FindByEmailAsync(loginRequestDto.Email);
            if (identityUser != null)
            {
                var checkPasswordResult = await _userManager.CheckPasswordAsync(identityUser, loginRequestDto.Password);
                if (checkPasswordResult)
                {
                    var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(identityUser);
                    return Ok(tokens);
                }
            }
            return BadRequest("Username or password is incorrect");
        }


        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] TokenRequestDto tokenRequestDto)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid request");

            try
            {
                var result = await _unitOfWork.TokenRepository.RefreshTokensAsync(tokenRequestDto);
                return Ok(result);
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(new { Error = ex.Message });
            }
        }


        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequestDto)
        {
            if (string.IsNullOrEmpty(logoutRequestDto.RefreshToken))
            {
                return BadRequest("Refresh Token is required.");
            }

            var success = await _unitOfWork.TokenRepository.LogoutAsync(logoutRequestDto.RefreshToken);

            if (success)
            {
                return Ok(new { Message = "Successfully logged out." });
            }

            // Trường hợp này hiếm khi xảy ra, nhưng vẫn có thể có
            return BadRequest(new { Error = "Unable to log out." });
        }


        [HttpPost]
        [Route("google-login")]
        [AllowAnonymous] 
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        {
            if (string.IsNullOrEmpty(request.IdToken))
            {
                return BadRequest("Google ID Token is required.");
            }

            try
            {
                // 4. Lấy Client ID từ configuration thay vì hard-code
                var clientId = _configuration["Authentication:Google:ClientId"];

                if (string.IsNullOrEmpty(clientId))
                {
                    // Log lỗi và trả về 500 vì server cấu hình thiếu
                    // Log.Error("Google ClientId is not configured in appsettings.json");
                    return StatusCode(StatusCodes.Status500InternalServerError, "Authentication is not configured correctly.");
                }
  
                // 1. Xác thực IdToken với Google
                var validationSettings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { clientId },
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);

                // 2. Kiểm tra xem người dùng đã tồn tại trong DB chưa
                var user = await _userManager.FindByEmailAsync(payload.Email);

                // 3. Nếu người dùng đã tồn tại -> Đăng nhập và trả về token
                if (user != null)
                {
                    // Kiểm tra xem user này đã liên kết với Google login chưa
                    var userLogins = await _userManager.GetLoginsAsync(user);
                    var googleLogin = userLogins.FirstOrDefault(l => l.LoginProvider == "Google" && l.ProviderKey == payload.Subject);

                    if (googleLogin == null)
                    {
                        // Nếu chưa, thêm thông tin đăng nhập Google vào cho user đã có
                        await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
                    }

                    // Tạo token của hệ thống và trả về
                    var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(user);
                    return Ok(tokens);
                }

                // 4. Nếu người dùng chưa tồn tại -> Tạo người dùng mới
                var newUser = new ApplicationUser
                {
                    Email = payload.Email,
                    UserName = payload.Email, // Hoặc payload.Name
                    EmailConfirmed = payload.EmailVerified // Dùng trạng thái xác thực từ Google
                };

                var identityResult = await _userManager.CreateAsync(newUser);

                if (identityResult.Succeeded)
                {
                    // Gán role mặc định cho người dùng đăng nhập bằng Google
                    // Lưu ý: Không nên gán "Admin" làm mặc định
                    await _userManager.AddToRoleAsync(newUser, "User"); 

                    // Liên kết tài khoản mới với nhà cung cấp Google
                    await _userManager.AddLoginAsync(newUser, new UserLoginInfo("Google", payload.Subject, "Google"));

                    // Tạo token của hệ thống và trả về
                    var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(newUser);
                    return Ok(tokens);
                }

                // Nếu tạo user thất bại
                var errorMessages = identityResult.Errors.Select(e => e.Description);
                return BadRequest($"User creation failed: {string.Join(", ", errorMessages)}");
            }
            catch (InvalidJwtException ex)
            {
                return BadRequest($"Invalid Google Token: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }
    }
}
