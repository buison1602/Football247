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
    public class CreateUserCommand : CreateUserCommandModel, IRequest<MethodResult<UserDto>>
    {
    }

    public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, MethodResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;
        private readonly IMapper _mapper;
        
        public CreateUserCommandHandler(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }

        public async Task<MethodResult<UserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<UserDto>();

            #region Validation
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
            #endregion

            if (!await _roleManager.RoleExistsAsync(request.Role))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Role), request.Role);
                return methodResult;
            }

            var newUser = new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
            };

            var result = await _userManager.CreateAsync(newUser, request.Password);

            if (!result.Succeeded)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), result.Errors.ToString());
                return methodResult;
            }

            await _userManager.AddToRoleAsync(newUser, request.Role);

            methodResult.Result = _mapper.Map<UserDto>(newUser);
            return methodResult;
        }
    }
}
