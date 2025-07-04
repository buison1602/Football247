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

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly Football247AuthDbContext _authDbContext;
        private readonly IConfiguration _configuration;

        public AuthController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork, 
            Football247AuthDbContext authDbContext, IConfiguration configuration)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
            this._authDbContext = authDbContext;
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

            Console.WriteLine("/n/n------------------------------/n/n");
            Console.WriteLine("identityUser  " + identityUser.Email);
            Console.WriteLine("identityUser  " + identityUser.UserName);
            Console.WriteLine("identityUser  " + registerRequestDto.Password);
            Console.WriteLine("/n/n------------------------------/n/n");


            var identityResult = await _userManager.CreateAsync(identityUser, registerRequestDto.Password);

            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRoleAsync(identityUser, "User");

                if (identityResult.Succeeded)
                {
                    return Ok("User registered successfully. Please login.");
                }
            }

            return BadRequest("User registration failed. Please try again.");
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
    }
}
