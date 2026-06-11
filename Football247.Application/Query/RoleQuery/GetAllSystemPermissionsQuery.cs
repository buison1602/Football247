using Football247.Authorization;
using Football247.Domain.Models.EntityModels.DTOs.Role;
using MediatR;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.Role
{
    public class GetAllSystemPermissionsQuery : IRequest<MethodResult<List<string>>>
    {
    }

    public class GetAllSystemPermissionsQueryHandler : IRequestHandler<GetAllSystemPermissionsQuery, MethodResult<List<string>>>
    {

        public async Task<MethodResult<List<string>>> Handle(GetAllSystemPermissionsQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<string>>();

            methodResult.Result = Permissions.GetAllPermissions();
            return methodResult;
        }
    }
}
