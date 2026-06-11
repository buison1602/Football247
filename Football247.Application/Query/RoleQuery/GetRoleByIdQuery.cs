using Football247.Authorization;
using Football247.Domain.Models.EntityModels.DTOs.Role;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.Role
{
    public class GetRoleByIdQueryModel
    {
        public Guid Id { get; set; }
    }

    public class GetRoleByIdQuery : GetRoleByIdQueryModel, IRequest<MethodResult<RoleDto>>
    {
    }

    public class GetRoleByIdHandler : IRequestHandler<GetRoleByIdQuery, MethodResult<RoleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRoleByIdHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<RoleDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<RoleDto>();

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(request.Id);
            if (role == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }

            var claims = await _unitOfWork.RoleRepository.GetClaimsAsync(role);
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = claims
                    .Where(c => c.Type == CustomClaimTypes.Permission)
                    .Select(c => c.Value)
                    .ToList()
            };

            methodResult.Result = roleDto;
            return methodResult;
        }
    }
}
