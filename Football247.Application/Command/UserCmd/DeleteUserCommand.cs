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
    public class DeleteUserCommand : DeleteUserCommandModel, IRequest<MethodResult<UserDto>>
    {
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, MethodResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public DeleteUserCommandHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MethodResult<UserDto>> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<UserDto>();

            var user = await _userManager.FindByIdAsync(request.Id.ToString());

            if (user == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id.ToString());
                return methodResult;
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), string.Join(", ", result.Errors.Select(e => e.Description)));
                return methodResult;
            }

            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
