using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.SignalR
{
    /// <summary>
    /// Hub xử lý realtime cho football data:
    /// matches, standings.
    /// AllowAnonymous vì guest cũng xem được lịch thi đấu.
    /// </summary>
    [AllowAnonymous]
    public class FootballHub : BaseHub
    {
        protected override async Task OnHubConnectedAsync()
        {
            // Client kết nối kèm query param: ?competition=PL
            var competition = Context.GetHttpContext()
                ?.Request.Query["competition"].ToString().ToUpper();

            if (!string.IsNullOrEmpty(competition))
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, $"{competition}-matches");
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, $"{competition}-standings");
            }
        }

        protected override async Task OnHubDisconnectedAsync(Exception? exception)
        {
            var competition = Context.GetHttpContext()
                ?.Request.Query["competition"].ToString().ToUpper();

            if (!string.IsNullOrEmpty(competition))
            {
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId, $"{competition}-matches");
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId, $"{competition}-standings");
            }
        }
    }
}
