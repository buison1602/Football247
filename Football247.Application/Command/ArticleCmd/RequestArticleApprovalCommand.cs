using Football247.Application.Common.Data;
using Football247.Application.SignalR;
using Football247.Domain.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Shared.Enum;
using Shared.Response;
using System.Security.Claims;
using Shared.Enum.EnumNotification;

namespace Football247.Application.Command.ArticleCmd
{
    public class RequestApproveArticleCommandModel
    {
        public Guid ArticleId { get; set; }
    }

    public class RequestArticleApprovalCommand : RequestApproveArticleCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class RequestArticleApprovalCommandHandler : IRequestHandler<RequestArticleApprovalCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestArticleApprovalCommandHandler(IUnitOfWork unitOfWork, IHubContext<NotificationHub> hubContext, IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<MethodResult<bool>> Handle(RequestArticleApprovalCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var article = await _unitOfWork.ArticleRepository.GetByIdAsync(request.ArticleId);
            if (article == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.ArticleId), request.ArticleId);
                methodResult.StatusCode = StatusCodes.Status404NotFound;
                return methodResult;
            }
            
            // Approve the article
            article.Status = EnumStatusArticle.Pending;
            var articleDomain = await _unitOfWork.ArticleRepository.UpdateAsync(request.ArticleId, article);
            
            if (articleDomain == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), "Failed to update article status.");
                methodResult.StatusCode = StatusCodes.Status500InternalServerError;
                return methodResult;
            }

            // Lấy userId từ JWT Token
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid senderId);

            // Tạo mới Notification
            await _unitOfWork.NotificationMessageRepository.CreateAsync(new NotificationMessage
            {
                Id = Guid.NewGuid(),
                SenderId = senderId,
                Title = "Article Approval Request",
                Message = $"An article titled '{articleDomain.Title}' is pending approval.",
                NotificationType = EnumNotificationType.RequestReviewArticle,
            });

            // Notify users about the article approval request
            await _hubContext.Clients.Group($"role_{Roles.Admin}").SendAsync("ArticleApprovalRequested", articleDomain);

            await _unitOfWork.SaveAsync();

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
