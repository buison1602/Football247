using Football247.Application.Service.UserService;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Football247.Infrastructure.Services.UserService
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var userIdString = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
                return Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty;
            }
        }

        public string FullName
        {
            get
            {
                return _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Name) ?? "System";
            }
        }
    }
}
