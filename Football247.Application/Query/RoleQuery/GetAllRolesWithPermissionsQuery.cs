using Football247.Authorization;
using Football247.Domain.Models.EntityModels.DTOs.Role;
using Football247.Repositories.IRepository;
using MediatR;
using Shared.Response;

namespace Football247.Application.Query.Role
{
    public class GetAllRolesWithPermissionsQuery : IRequest<MethodResult<List<RoleDto>>>
    {
    }

    public class GetAllRolesWithPermissionsHandler : IRequestHandler<GetAllRolesWithPermissionsQuery, MethodResult<List<RoleDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllRolesWithPermissionsHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<RoleDto>>> Handle(GetAllRolesWithPermissionsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<RoleDto>>();

            var roles = await _unitOfWork.RoleRepository.GetAllAsync();
            var roleDtos = new List<RoleDto>();

            foreach (var role in roles)
            {
                var claims = await _unitOfWork.RoleRepository.GetClaimsAsync(role);
                roleDtos.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = claims
                        .Where(c => c.Type == CustomClaimTypes.Permission)
                        .Select(c => c.Value)
                        .ToList()
                });
            }

            methodResult.Result = roleDtos;

            return methodResult;
        }
    }
}
