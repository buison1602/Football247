using AutoMapper;
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

namespace Football247.Application.Query.UserQuery
{
    public class GetUserByIdQuery : IRequest<MethodResult<UserDto>>
    {
        public Guid Id { get; set; }
    }

    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, MethodResult<UserDto>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetUserByIdQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<MethodResult<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<UserDto>();
            
            var user = await _userManager.FindByIdAsync(request.Id.ToString());
            
            if (user == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id.ToString());
                return methodResult;
            }

            methodResult.StatusCode = 200;
            methodResult.Result = _mapper.Map<UserDto>(user);
            return methodResult;
        }
    }
}
