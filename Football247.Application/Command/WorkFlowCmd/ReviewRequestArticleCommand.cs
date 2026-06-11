using Football247.Application.Command.SendEmailCmd;
using Football247.Application.SignalR;
using Football247.Domain.Entities;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shared.Enum;
using Shared.Enum.EnumNotification;
using Shared.Response;
using System.Security.Claims;

namespace Football247.Application.Command.WorkFlowCmd
{
    public class ReviewRequestArticleCommandModel
    {
        public Guid ArticleId { get; set; }
        public EnumStatusArticle StatusArticle { get; set; }
        public string? Comment { get; set; }
    }

    public class ReviewRequestArticleCommand : ReviewRequestArticleCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class ReviewRequestArticleCommandHandler : IRequestHandler<ReviewRequestArticleCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration; 

        public ReviewRequestArticleCommandHandler(IUnitOfWork unitOfWork, 
            IHubContext<NotificationHub> hubContext, 
            IHttpContextAccessor httpContextAccessor, 
            IMediator mediator, 
            IConfiguration configuration,
            UserManager<ApplicationUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task<MethodResult<bool>> Handle(ReviewRequestArticleCommand request, CancellationToken cancellationToken)
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

            // Lấy userId từ JWT Token
            var userIdStr = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            Guid.TryParse(userIdStr, out Guid senderId);

            var notification = new NotificationMessage
            {
                Id = Guid.NewGuid(),
                UserId = article.CreatedUserId,
                SenderId = senderId,
                Message = request.Comment ?? string.Empty,
                NotificationType = EnumNotificationType.ReviewRequestArticle,
            };

            // Reject the article
            if (request.StatusArticle == EnumStatusArticle.Rejected) 
            {
                article.Status = EnumStatusArticle.Rejected;
                article.IsApproved = false;
                notification.Title = "Your article has been rejected";
            }
            // Approve the article
            else if (request.StatusArticle == EnumStatusArticle.Approved)
            {
                article.Status = EnumStatusArticle.Approved;
                notification.Title = "Your article has been approved";
                article.IsApproved = true;

                // Gửi notification tới tất cả User đang bật nhận thông báo 
                var publicNotification = new NotificationMessage
                {
                    Id = Guid.NewGuid(),
                    UserId = null,     
                    SenderId = null,   
                    Title = "Có bài viết mới",
                    Message = article.Title,
                    RedirectUrl = $"/article/{article.Slug}", // Gắn tạm đường dẫn tương đối
                    ObjectId = article.Id,
                    NotificationType = EnumNotificationType.NewArticle
                };

                await _unitOfWork.NotificationMessageRepository.CreateAsync(publicNotification);

                await _hubContext.Clients.All.SendAsync("ReceiveGlobalNotification", publicNotification);

                try
                {
                    // Lấy danh sách email những người bật nhận thông báo
                    // Giả sử bảng User của bạn có field IsReceiveEmailNotification
                    var subscriberEmails = await _userManager.Users
                        .Where(u => u.ReceiveEmailNotifications)
                        .Select(u => u.Email)
                        .ToListAsync();

                    if (subscriberEmails != null && subscriberEmails.Any())
                    {
                        // Bắn Message gửi mail qua Mediator (Xử lý bất đồng bộ hoặc dùng Queue như đã bàn)
                        var emailResult = _mediator.Send(new SendEmailByTemplateCommand
                        {
                            Template = EnumSenderTemplate.SendNewArticleNotification,
                            Subject = $"[Football247] {article.Title}",
                            ToEmails = subscriberEmails,
                            Params = new
                            {
                                Title = article.Title,
                                Description = article.Description,
                                Slug = article.Slug,
                                BaseUrl = _configuration["AppConfig:BaseUrl"] ?? "https://football247.vn"
                            }
                        }, cancellationToken);

                        if (!emailResult.IsCompleted)
                        {
                            Console.WriteLine($"\n\n LỖI GỬI MAIL: {emailResult.Exception} - {emailResult.Result} \n\n");
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Không return lỗi ở đây để tránh làm gián đoạn việc Approve bài viết
                }
            }
            else
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.InValidFormat), nameof(request.StatusArticle), request.StatusArticle);
                methodResult.StatusCode = StatusCodes.Status400BadRequest;
                return methodResult;
            }

            var articleDomain = await _unitOfWork.ArticleRepository.UpdateAsync(request.ArticleId, article);

            await _unitOfWork.NotificationMessageRepository.CreateAsync(notification);

            await _hubContext.Clients.Group($"user_{article.CreatedUserId}").SendAsync("ArticleApprovalResult", articleDomain);

            await _unitOfWork.SaveAsync();

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}