using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Enum;
using Shared.Response;

namespace Football247.Application.Query.StoreQuery.OrderQuery
{
    public class GetOrdersAdminQuery : BaseQueryModel, IRequest<MethodResult<PagingItemsModel<OrderSummaryDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public EnumOrderStatuss? StatusFilter { get; set; }
        public EnumPaymentStatus? PaymentStatusFilter { get; set; }
        public EnumPaymentMethodd? PaymentMethodFilter { get; set; }
        public string? SearchKeyword { get; set; } // tìm theo OrderCode, ReceiverName, PhoneNumber
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class GetOrdersAdminQueryHandler : IRequestHandler<GetOrdersAdminQuery, MethodResult<PagingItemsModel<OrderSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetOrdersAdminQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<PagingItemsModel<OrderSummaryDto>>> Handle(GetOrdersAdminQuery request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<PagingItemsModel<OrderSummaryDto>>();

            var query = _unitOfWork.OrderRepository.ReadQueryable
                .Include(o => o.OrderItems)
                .AsQueryable();

            if (request.StatusFilter.HasValue)
                query = query.Where(o => o.Status == request.StatusFilter.Value);

            if (request.PaymentStatusFilter.HasValue)
                query = query.Where(o => o.PaymentStatus == request.PaymentStatusFilter.Value);

            if (request.PaymentMethodFilter.HasValue)
                query = query.Where(o => o.PaymentMethod == request.PaymentMethodFilter.Value);

            if (!string.IsNullOrEmpty(request.SearchKeyword))
                query = query.Where(o =>
                    o.OrderCode.Contains(request.SearchKeyword) ||
                    o.ReceiverName.Contains(request.SearchKeyword) ||
                    o.PhoneNumber.Contains(request.SearchKeyword));

            if (request.FromDate.HasValue)
                query = query.Where(o => o.CreatedDate >= request.FromDate.Value);

            if (request.ToDate.HasValue)
                query = query.Where(o => o.CreatedDate <= request.ToDate.Value.AddDays(1));

            var totalItems = await query.CountAsync(cancellationToken);

            var orders = await query
                .OrderByDescending(o => o.CreatedDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(o => new OrderSummaryDto
                {
                    Id = o.Id,
                    OrderCode = o.OrderCode,
                    TotalAmount = o.TotalAmount,
                    Status = o.Status.ToString(),
                    PaymentStatus = o.PaymentStatus.ToString(),
                    PaymentMethod = o.PaymentMethod.ToString(),
                    CreatedDate = o.CreatedDate,
                    ItemCount = o.OrderItems.Count,
                    ReceiverName = o.ReceiverName,
                    PhoneNumber = o.PhoneNumber
                })
                .ToListAsync(cancellationToken);

            methodResult.Result = new PagingItemsModel<OrderSummaryDto>(orders, request, totalItems);

            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}