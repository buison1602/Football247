using Football247.Application.Common.Data;
using Football247.Domain.Models.CommandModels.AuthCommand;
using Football247.Domain.Models.EntityModels.DTOs.Auth;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared.Response;


namespace Football247.Application.Command.AuthCmd
{
    public class RegisterCommand : RegisterCommandModel, IRequest<MethodResult<AuthResultDto>>
    {
    }

    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, MethodResult<AuthResultDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public RegisterCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<AuthResultDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<AuthResultDto>();

            #region Validate

            if (request.Email == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Email));
                return methodResult;
            }

            var existingUser = await _userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Email), request.Email);
                return methodResult;
            }

            if (request.Password == null || request.Password.Length < 6)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Min), nameof(request.Password), "Password must be at least 6 characters long.");
                return methodResult;
            }

            if (request.ConfirmPassword == null || request.ConfirmPassword.Length < 6)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Min), nameof(request.ConfirmPassword), "Password must be at least 6 characters long.");
                return methodResult;
            }

            if (request.Password != request.ConfirmPassword)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.InValidFormat), "Password and Confirm Password do not match.");
                return methodResult;
            }

            if (request.UserName == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.UserName));
                return methodResult;
            }

            #endregion

            var identityUser = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName
            };

            var identityResult = await _userManager.CreateAsync(identityUser, request.Password);
            if (identityResult.Succeeded)
            {
                identityResult = await _userManager.AddToRoleAsync(identityUser, Roles.User);

                if (identityResult.Succeeded)
                {
                    var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(identityUser);

                    methodResult.Result = new AuthResultDto
                    {
                        UserId = tokens.UserId.ToString(),
                        FullName = tokens.FullName,
                        JwtToken = tokens.JwtToken,
                        RefreshToken = tokens.RefreshToken
                    };
                    
                    return methodResult;
                }
                else
                {
                    methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.InValidFormat), "An error occurred while creating the user.");
                    return methodResult;
                }
            }

            methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.InValidFormat), "An error occurred while creating the user.");
            
            return methodResult;
        }
    }
}
