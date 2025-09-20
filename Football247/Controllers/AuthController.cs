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
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using Football247.Services;
using Football247.Services.IService;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly IAuthService _authService;

        public AuthController(UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IAuthService authService,
            IConfiguration configuration)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
            this._configuration = configuration;
            this._authService = authService;
        }


        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
        {
            var result = await _authService.RegisterAsync(registerRequestDto);

            if (!result.Succeeded)
            {
                return BadRequest($"User registration failed: {string.Join(", ", result.Errors)}");
            }

            SetRefreshTokenInCookie(result.RefreshToken);

            return Ok(new
            {
                userId = result.UserId,
                fullName = result.FullName,
                jwtToken = result.JwtToken
            });
        }


        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
        {
            var result = await _authService.LoginAsync(loginRequestDto);

            if (result == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            SetRefreshTokenInCookie(result.RefreshToken);

            return Ok(new
            {
                userId = result.UserId,
                fullName = result.FullName,
                jwtToken = result.JwtToken
            });
        }


        [HttpPost]
        [Route("Refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh Token not found.");
            }

            try
            {
                var result = await _authService.RefreshTokenAsync(refreshToken);
                if (!result.Succeeded)
                {
                    return Unauthorized("Invalid refresh token.");
                }

                SetRefreshTokenInCookie(result.RefreshToken);

                return Ok(new
                {
                    userId = result.UserId,
                    fullName = result.FullName,
                    jwtToken = result.JwtToken
                });
            }
            catch (ArgumentNullException)
            {
                return BadRequest("Refresh Token is required.");
            }
            catch (SecurityTokenException)
            {
                return Unauthorized("Invalid refresh token.");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
            }
        }

        [HttpPost]
        [Route("Logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
            {
                return BadRequest("Refresh Token is required.");
            }

            var success = await _unitOfWork.TokenRepository.LogoutAsync(refreshToken);

            if (success)
            {
                Response.Cookies.Delete("refreshToken");
                return Ok(new { Message = "Successfully logged out." });
            }

            return BadRequest(new { Error = "Unable to log out." });
        }


        //[HttpPost]
        //[Route("google-login")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequestDto request)
        //{
        //    if (string.IsNullOrEmpty(request.IdToken))
        //    {
        //        return BadRequest("Google ID Token is required.");
        //    }

        //    try
        //    {
        //        // 4. Lấy Client ID từ configuration thay vì hard-code
        //        var clientId = _configuration["Authentication:Google:ClientId"];

        //        if (string.IsNullOrEmpty(clientId))
        //        {
        //            // Log lỗi và trả về 500 vì server cấu hình thiếu
        //            // Log.Error("Google ClientId is not configured in appsettings.json");
        //            return StatusCode(StatusCodes.Status500InternalServerError, "Authentication is not configured correctly.");
        //        }

        //        // 1. Xác thực IdToken với Google
        //        var validationSettings = new GoogleJsonWebSignature.ValidationSettings()
        //        {
        //            Audience = new[] { clientId },
        //        };

        //        var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken, validationSettings);

        //        // 2. Kiểm tra xem người dùng đã tồn tại trong DB chưa
        //        var user = await _userManager.FindByEmailAsync(payload.Email);

        //        // 3. Nếu người dùng đã tồn tại -> Đăng nhập và trả về token
        //        if (user != null)
        //        {
        //            // Kiểm tra xem user này đã liên kết với Google login chưa
        //            var userLogins = await _userManager.GetLoginsAsync(user);
        //            var googleLogin = userLogins.FirstOrDefault(l => l.LoginProvider == "Google" && l.ProviderKey == payload.Subject);

        //            if (googleLogin == null)
        //            {
        //                // Nếu chưa, thêm thông tin đăng nhập Google vào cho user đã có
        //                await _userManager.AddLoginAsync(user, new UserLoginInfo("Google", payload.Subject, "Google"));
        //            }

        //            // Tạo token của hệ thống và trả về
        //            var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(user);
        //            return Ok(tokens);
        //        }

        //        // 4. Nếu người dùng chưa tồn tại -> Tạo người dùng mới
        //        var newUser = new ApplicationUser
        //        {
        //            Email = payload.Email,
        //            UserName = payload.Email, // Hoặc payload.Name
        //            EmailConfirmed = payload.EmailVerified // Dùng trạng thái xác thực từ Google
        //        };

        //        var identityResult = await _userManager.CreateAsync(newUser);

        //        if (identityResult.Succeeded)
        //        {
        //            // Gán role mặc định cho người dùng đăng nhập bằng Google
        //            // Lưu ý: Không nên gán "Admin" làm mặc định
        //            await _userManager.AddToRoleAsync(newUser, "User");

        //            // Liên kết tài khoản mới với nhà cung cấp Google
        //            await _userManager.AddLoginAsync(newUser, new UserLoginInfo("Google", payload.Subject, "Google"));

        //            // Tạo token của hệ thống và trả về
        //            var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(newUser);
        //            return Ok(tokens);
        //        }

        //        // Nếu tạo user thất bại
        //        var errorMessages = identityResult.Errors.Select(e => e.Description);
        //        return BadRequest($"User creation failed: {string.Join(", ", errorMessages)}");
        //    }
        //    catch (InvalidJwtException ex)
        //    {
        //        return BadRequest($"Invalid Google Token: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occurred.");
        //    }
        //}



        private void SetRefreshTokenInCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7),
                Secure = false, // Chỉ true nếu dùng HTTPS
                SameSite = SameSiteMode.Lax
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);
        }
    }
}

