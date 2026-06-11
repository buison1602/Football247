using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;

namespace Football247.Application.SignalR
{
    /// <summary>
    /// Hub xử lý comment realtime cho từng bài viết.
    /// Client kết nối kèm query param: ?articleId={guid}
    /// → tự động join group "article_{articleId}"
    /// → chỉ nhận event ReceiveComment của bài viết đó.
    /// </summary>
    [AllowAnonymous] // Guest cũng có thể xem comment realtime
    public class ArticleCommentHub : BaseHub
    {
        protected override async Task OnHubConnectedAsync()
        {
            var articleId = Context.GetHttpContext()?.Request.Query["articleId"].ToString();
            if (!string.IsNullOrEmpty(articleId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, $"article_{articleId.ToLower()}");
            }
        }

        protected override async Task OnHubDisconnectedAsync(Exception? exception)
        {
            var articleId = Context.GetHttpContext()?.Request.Query["articleId"].ToString();
            if (!string.IsNullOrEmpty(articleId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"article_{articleId.ToLower()}");
            }

            await base.OnHubDisconnectedAsync(exception);
        }
    }
}
