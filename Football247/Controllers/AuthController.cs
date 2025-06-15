using Football247.Models.DTOs.Auth;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public AuthController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            this._userManager = userManager;
            this._unitOfWork = unitOfWork;
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
                    var roles = await _userManager.GetRolesAsync(identityUser);

                    if (roles != null)
                    {
                        var jwtToken = _unitOfWork.TokenRepository.CreateJWTToken(identityUser, roles.ToList());

                        var response = new LoginResponseDto
                        {
                            UserId = identityUser.Id,
                            FullName = identityUser.UserName,
                            JwtToken = jwtToken,
                        };

                        return Ok(response);
                    }
                }
            }

            return BadRequest("Username or password is incorrect");
        }
    }
}
