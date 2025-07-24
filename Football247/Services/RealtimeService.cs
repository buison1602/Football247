using Football247.Services.IService;
using Football247.SignalR;
using Microsoft.AspNetCore.SignalR;

namespace Football247.Services
{
    public class RealtimeService : IRealtimeService
    {
        private readonly IHubContext<Football247Hub> _hubContext;

        public RealtimeService(IHubContext<Football247Hub> hubContext)
        {
            _hubContext = hubContext;
        }

        public Task NotifyAllAsync(string method, object data)
        {
            return _hubContext.Clients.All.SendAsync(method, data);
        }

        public Task NotifyUserAsync(string userId, string method, object data)
        {
            return Football247Hub.SendToUser(userId, method, data, _hubContext);
        }
    }

}
