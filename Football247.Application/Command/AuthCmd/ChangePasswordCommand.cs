using Football247.Models.Entities;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Football247.Authorization.Permissions;

namespace Football247.Application.Command.AuthCmd
{
    public class ChangePasswordCommand : IRequest<MethodResult<bool>>
    {
        public Guid UserId { get; set; }
        public string? NewPassword { get; set; }
        public string? CurrentPassword { get; set; }
    }

    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, MethodResult<bool>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;


        public ChangePasswordCommandHandler(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        public async Task<MethodResult<bool>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentNullException.ThrowIfNull(request.NewPassword);
            ArgumentNullException.ThrowIfNull(request.CurrentPassword);
            MethodResult<bool> methodResult = new MethodResult<bool>();

            ApplicationUser? user = await _userManager.FindByIdAsync(request.UserId.ToString());

            if (user == null)
            {
                methodResult.AddError(
                    StatusCodes.Status400BadRequest,
                    nameof(EnumSystemErrorCode.UserNotExist),
                    nameof(request.CurrentPassword), request.CurrentPassword);
                methodResult.Result = false;
                return methodResult;
            }
            if (request.CurrentPassword == request.NewPassword)
            {
                methodResult.AddError(
                    StatusCodes.Status400BadRequest,
                    nameof(EnumSystemErrorCode.NewPasswordMustBeDiffirent),
                    nameof(request.CurrentPassword));
                methodResult.Result = false;
                return methodResult;
            }

            var checkOldPassword = await _signInManager.PasswordSignInAsync(user.UserName ?? string.Empty, request.CurrentPassword, false, false);

            if (!checkOldPassword.Succeeded)
            {
                methodResult.AddError(
                    StatusCodes.Status400BadRequest,
                    nameof(EnumSystemErrorCode.ErrorCurrentPassword),
                    nameof(request.CurrentPassword));
                methodResult.Result = false;
                return methodResult;
            }


            var result = await _userManager.PasswordValidators.First().ValidateAsync(_userManager, user, request.NewPassword);
            if (!result.Succeeded)
            {
                methodResult.AddError(
                    StatusCodes.Status400BadRequest,
                    nameof(EnumSystemErrorCode.InvalidPassword),
                    nameof(request.NewPassword));
                methodResult.Result = false;
                return methodResult;
            }

            await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
