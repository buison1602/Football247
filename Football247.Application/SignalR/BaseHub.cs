using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Football247.Application.SignalR
{
    public abstract class BaseHub : Hub
    {
        // Track connectionId theo userId cho toàn bộ Hub
        protected static readonly ConnectionMapping<string> _userConnections = new();

        public sealed override async Task OnConnectedAsync()
        {
            try
            {
                var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                Console.WriteLine($"[HUB CONNECTED] ConnectionId={Context.ConnectionId}, UserId={userId}");

                if (userId != null)
                    _userConnections.Add(userId, Context.ConnectionId);

                await OnHubConnectedAsync();
                await base.OnConnectedAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[HUB ERROR] OnConnectedAsync failed: {ex}");
                throw; // re-throw để SignalR trả về type:7 với message
            }
        }

        public sealed override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _userConnections.Remove(userId, Context.ConnectionId);
            }

            await OnHubDisconnectedAsync(exception);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Override để thêm logic khi client kết nối (ví dụ: join group theo articleId).
        /// </summary>
        protected virtual Task OnHubConnectedAsync() => Task.CompletedTask;

        /// <summary>
        /// Override để thêm logic khi client ngắt kết nối (ví dụ: leave group).
        /// </summary>
        protected virtual Task OnHubDisconnectedAsync(Exception? exception) => Task.CompletedTask;

        /// <summary>
        /// Helper: lấy tất cả connectionId của một userId.
        /// Dùng khi cần gửi message trực tiếp qua IHubContext.
        /// </summary>
        public static IEnumerable<string> GetConnectionIds(string userId)
            => _userConnections.GetConnections(userId);
    }
}
