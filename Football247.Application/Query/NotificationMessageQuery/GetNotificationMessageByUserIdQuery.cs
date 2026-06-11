using Football247.Domain.Entities;
using Football247.Domain.Models.EntityModels.DTOs.Notification;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.NotificationMessageQuery
{
    public class GetNotificationMessageByUserIdQueryModel : BaseQueryModel
    {
        public Guid UserId { get; set; }
    }

    public class GetNotificationMessageByUserIdQuery : GetNotificationMessageByUserIdQueryModel, IRequest<MethodResult<PagingItemsModel<NotificationMessageDto>>>
    {
    }

    public class GetNotificationMessageByUserIdQueryHandler : IRequestHandler<GetNotificationMessageByUserIdQuery, MethodResult<PagingItemsModel<NotificationMessageDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetNotificationMessageByUserIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<PagingItemsModel<NotificationMessageDto>>> Handle(GetNotificationMessageByUserIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<NotificationMessageDto>>();
            
            var query = _unitOfWork.NotificationMessageRepository.ReadQueryable
                .Where(n => n.UserId == request.UserId || n.UserId == null)
                .OrderByDescending(n => n.CreatedDate);
            
            var totalItems = await query.CountAsync(cancellationToken);
            
            var notifications = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var notificationDtos = notifications.Select(n => new NotificationMessageDto
            {
                UserId = n.UserId,
                Title = n.Title,
                Message = n.Message,
                RedirectUrl = n.RedirectUrl,
                ObjectId = n.ObjectId,
                NotificationType = n.NotificationType,
                CreatedDate = n.CreatedDate
            }).ToList();

            methodResult.Result = new PagingItemsModel<NotificationMessageDto> (notificationDtos, request, totalItems);
            return methodResult;
        }
    }
}
