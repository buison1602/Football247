using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace Football247.SignalR
{
    [Authorize] 
    public class Football247Hub : Hub
    {
        private static readonly ConnectionMapping<string> _connections = new();

        public override Task OnConnectedAsync()
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _connections.Add(userId, Context.ConnectionId);
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userId != null)
            {
                _connections.Remove(userId, Context.ConnectionId);
            }

            return base.OnDisconnectedAsync(exception);
        }

        // Hàm dùng để gửi dữ liệu đến 1 user cụ thể (ví dụ: cập nhật point)
        public static async Task SendToUser(string userId, string method, object data, IHubContext<Football247Hub> hubContext)
        {
            var connections = _connections.GetConnections(userId);
            foreach (var connId in connections)
            {
                await hubContext.Clients.Client(connId).SendAsync(method, data);
            }
        }
    }
}
