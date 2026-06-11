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
    public class LoginCommand : LoginCommandModel, IRequest<MethodResult<AuthResultDto>>
    {
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, MethodResult<AuthResultDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<AuthResultDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
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
            if (existingUser == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Email), request.Email);
                return methodResult;
            }

            if (request.Password == null || request.Password.Length < 6)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Min), nameof(request.Password), "Password must be at least 6 characters long.");
                return methodResult;
            }
            #endregion

            var checkPasswordResult = await _userManager.CheckPasswordAsync(existingUser, request.Password);
            if (!checkPasswordResult)
            {
                methodResult.AddError(StatusCodes.Status401Unauthorized, nameof(EnumSystemErrorCode.PermissionIsInvalid), "Invalid email or password.");
                return methodResult;
            }

            var tokens = await _unitOfWork.TokenRepository.CreateTokensAsync(existingUser);

            methodResult.Result = new AuthResultDto
            {
                UserId = tokens.UserId.ToString(),
                FullName = tokens.FullName,
                JwtToken = tokens.JwtToken,
                RefreshToken = tokens.RefreshToken
            };

            return methodResult;
        }
    }
}
