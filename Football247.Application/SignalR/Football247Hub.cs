using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Football247.Application.SignalR
{
    /// <summary>
    /// Hub chính dùng cho các tính năng realtime không thuộc domain cụ thể
    /// (ví dụ: ping/pong, broadcast toàn hệ thống, v.v.)
    /// ConnectionMapping và auth đã được xử lý ở BaseHub.
    /// </summary>
    [Authorize]
    public class Football247Hub : BaseHub
    {
        /// <summary>
        /// Gửi message đến tất cả connectionId của một user cụ thể.
        /// Dùng khi inject IHubContext<Football247Hub> từ bên ngoài.
        /// </summary>
        public static async Task SendToUserAsync(
            string userId,
            string method,
            object data,
            IHubContext<Football247Hub> hubContext)
        {
            var connections = GetConnectionIds(userId);
            foreach (var connId in connections)
            {
                await hubContext.Clients.Client(connId).SendAsync(method, data);
            }
        }
    }
}
