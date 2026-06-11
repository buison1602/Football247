using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Domain.Models.EntityModels.DTOs.User;
using Football247.Models.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Response;

namespace Football247.Application.Query.UserQuery
{
    public class GetAllUsersQuery : BaseQueryModel, IRequest<MethodResult<PagingItemsModel<UserDto>>>
    {
    }

    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, MethodResult<PagingItemsModel<UserDto>>>
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public GetAllUsersQueryHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<MethodResult<PagingItemsModel<UserDto>>> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<UserDto>>();

            var users = await _userManager.Users
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    UserName = u.UserName,
                    Email = u.Email,
                    AvatarUrl = u.AvatarUrl,
                    Points = u.Points,
                    SpinCount = u.SpinCount
                })
                .ToListAsync();
            var totalCount = await _userManager.Users.CountAsync();

            methodResult.StatusCode = 200;
            methodResult.Result = new PagingItemsModel<UserDto>(users, request, totalCount);
            return methodResult;
        }
    }
}
