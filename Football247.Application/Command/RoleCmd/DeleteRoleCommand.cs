using Football247.Application.Common.Data;
using Football247.Domain.Models.CommandModels.Role;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.RoleCmd
{
    public class DeleteRoleCommand : DeleteRoleCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class DeleteRoleCommandHandler : IRequestHandler<DeleteRoleCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;   

        public DeleteRoleCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(DeleteRoleCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(request.Id);
            if (role == null) 
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }

            if (role.Name == Roles.Admin)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.InValidFormat), nameof(role.Name), role.Name);
                return methodResult;
            }

            var result = await _unitOfWork.RoleRepository.DeleteAsync(role);

            if (result.Succeeded)
            {
                methodResult.Result = true;
            }
            else
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError,
                    nameof(EnumSystemErrorCode.ServerError), "Delete failed");
                methodResult.Result = false;
            }

            return methodResult;
        }
    }
}
