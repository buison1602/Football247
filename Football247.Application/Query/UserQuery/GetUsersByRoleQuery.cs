using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.User;
using Football247.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Response;

namespace Football247.Application.Query.UserQuery
{
    public class GetUsersByRoleQuery : BaseQueryModel, IRequest<MethodResult<PagingItemsModel<UserDto>>>
    {
        public string RoleName { get; set; } = string.Empty;
    }

    public class GetUsersByRoleQueryHandler : IRequestHandler<GetUsersByRoleQuery, MethodResult<PagingItemsModel<UserDto>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public GetUsersByRoleQueryHandler(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<MethodResult<PagingItemsModel<UserDto>>> Handle(GetUsersByRoleQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<UserDto>>();

            var users = await _userManager.GetUsersInRoleAsync(request.RoleName);
            var userDtos = _mapper.Map<List<UserDto>>(users);

            var totalCount = userDtos.Count();

            methodResult.StatusCode = 200;
            methodResult.Result = new PagingItemsModel<UserDto>(userDtos, request, totalCount);

            return methodResult;
        }
    }
}
