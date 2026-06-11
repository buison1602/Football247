using Football247.Authorization;
using Football247.Domain.Models.CommandModels.Role;
using Football247.Domain.Models.EntityModels.DTOs.Role;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;
using System.Security.Claims;

namespace Football247.Application.Command.RoleCmd
{
    public class UpdateRoleCommand : UpdateRoleCommandModel, IRequest<MethodResult<RoleDto>>
    {
    }

    public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, MethodResult<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<RoleDto>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<RoleDto>();

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(request.Id);
                if (role == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                    return methodResult;
                }

                role.Name = request.Name;
                var updateResult = await _unitOfWork.RoleRepository.UpdateAsync(role);
                if (!updateResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.ServerError), nameof(request.Permissions), request.Permissions);
                    return methodResult;
                }

                var currentClaims = await _unitOfWork.RoleRepository.GetClaimsAsync(role);
                var currentPermissions = currentClaims.Where(c => c.Type == CustomClaimTypes.Permission).ToList();

                foreach (var claim in currentPermissions)
                {
                    await _unitOfWork.RoleRepository.RemoveClaimAsync(role, claim);
                }

                var validSystemPermissions = Permissions.GetAllPermissions();
                foreach (var permissionName in request.Permissions)
                {
                    if (validSystemPermissions.Contains(permissionName))
                    {
                        await _unitOfWork.RoleRepository.AddClaimAsync(role,
                            new Claim(CustomClaimTypes.Permission, permissionName));
                    }
                }

                await _unitOfWork.CommitTransactionAsync();

                methodResult.Result = new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = request.Permissions
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
