using Football247.Authorization;
using Football247.Domain.Models.CommandModels.Role;
using Football247.Domain.Models.EntityModels.DTOs.Role;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Shared.Response;
using System.Security.Claims;

namespace Football247.Application.Command.RoleCmd
{
    public class CreateRoleCommand : CreateRoleCommandModel, IRequest<MethodResult<RoleDto>>
    {
    }

    public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, MethodResult<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<RoleDto>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<RoleDto>();

            if (request.Permissions != null && request.Permissions.Any())
            {
                var validSystemPermissions = Permissions.GetAllPermissions();

                var invalidPermissions = request.Permissions.Except(validSystemPermissions).ToList();
                if (invalidPermissions.Any())
                {
                    methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.PermissionIsInvalid), nameof(request.Permissions), request.Permissions);
                    return methodResult;
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var role = new IdentityRole<Guid>(request.Name);
                var result = await _unitOfWork.RoleRepository.CreateAsync(role);

                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.ServerError), nameof(request.Permissions), request.Permissions);
                    return methodResult;
                }

                if (request.Permissions != null && request.Permissions.Any())
                {
                    var validSystemPermissions = Permissions.GetAllPermissions();
                    foreach (var permissionName in request.Permissions)
                    {
                        if (validSystemPermissions.Contains(permissionName))
                        {
                            await _unitOfWork.RoleRepository.AddClaimAsync(role,
                                new Claim(CustomClaimTypes.Permission, permissionName));
                        }
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                methodResult.Result = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = request.Permissions ?? new List<string>()
                };
                return methodResult;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }
    }
}