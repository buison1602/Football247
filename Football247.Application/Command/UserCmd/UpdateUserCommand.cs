using AutoMapper;
using Football247.Domain.Models.CommandModels.UserCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.User;
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

namespace Football247.Application.Command.UserCmd
{
    public class UpdateUserCommand : UpdateUserCommandModel, IRequest<MethodResult<UserDto>>
    {
    }

    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, MethodResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UpdateUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MethodResult<UserDto>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<UserDto>();

            #region Validation
            if (request.Name == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Name));
                return methodResult;
            }

            //var existingUser = await _userManager.FindByNameAsync(request.Name);
            //if (existingUser != null)
            //{
            //    methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Name), request.Name);
            //    return methodResult;
            //}

            if (request.Name != null && request.Name.Length < 6)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Min), nameof(request.Name), "Name must be at least 6 characters long.");
                return methodResult;
            }

            if (request.AvatarUrl == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.AvatarUrl));
                return methodResult;
            }
            #endregion

            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id.ToString());
                return methodResult;
            }

            user.UserName = request.Name;
            user.AvatarUrl = request.AvatarUrl;
            user.ReceiveEmailNotifications = request.ReceiveEmailNotifications;
            user.ReceiveInAppNotifications = request.ReceiveInAppNotifications;
            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), "Failed to update user.");
                return methodResult;
            }

            methodResult.Result = new UserDto
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                AvatarUrl = user.AvatarUrl,
                Points = user.Points,
                SpinCount = user.SpinCount,
                ReceiveEmailNotifications = user.ReceiveEmailNotifications,
                ReceiveInAppNotifications = user.ReceiveInAppNotifications
            };
            return methodResult;
        }
    }
}
