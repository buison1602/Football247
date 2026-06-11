using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Football247.Application.Common.Data;

namespace Football247.Application.SignalR
{
    /// <summary>
    /// Hub xử lý notification realtime cho từng user và group theo role.
    /// Mỗi user sau khi kết nối sẽ tự động join group "user_{userId}"
    /// và các group "role_{roleName}" tương ứng với các role của user.
    /// </summary>
    [Authorize]
    public class NotificationHub : BaseHub
    {
        protected override async Task OnHubConnectedAsync()
        {
            // Join user group
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            // Join role groups
            var roles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value);
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"role_{role}");
                }
            }
        }

        protected override async Task OnHubDisconnectedAsync(Exception? exception)
        {
            // Leave user group
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
            }

            // Leave role groups
            var roles = Context.User?.FindAll(ClaimTypes.Role).Select(c => c.Value);
            if (roles != null && roles.Any())
            {
                foreach (var role in roles)
                {
                    await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"role_{role}");
                }
            }
        }
    }
}
