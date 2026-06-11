using Football247.Domain.Models.EntityModels.DTOs.Order;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.StoreQuery.OrderQuery
{
    public class GetMyOrdersQuery : IRequest<MethodResult<List<OrderSummaryDto>>>
    {
        public Guid UserId { get; set; }
        public bool? PaidOnly { get; set; }
    }

    public class GetMyOrdersQueryHandler : IRequestHandler<GetMyOrdersQuery, MethodResult<List<OrderSummaryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetMyOrdersQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<OrderSummaryDto>>> Handle(GetMyOrdersQuery request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<List<OrderSummaryDto>>();

            var query = _unitOfWork.OrderRepository.ReadQueryable
                .Where(o => o.UserId == request.UserId);

            if (request.PaidOnly == true)
            {
                query = query.Where(o => o.PaymentStatus == EnumPaymentStatus.Paid);
            }
            else
            {
                query = query.Where(o => o.PaymentStatus == EnumPaymentStatus.Unpaid);
            }

                var orders = await query
                    .Include(o => o.OrderItems)
                    .OrderByDescending(o => o.CreatedDate)
                    .Select(o => new OrderSummaryDto
                    {
                        Id = o.Id,
                        OrderCode = o.OrderCode,
                        TotalAmount = o.TotalAmount,
                        Status = o.Status.ToString(),
                        PaymentStatus = o.PaymentStatus.ToString(),
                        PaymentMethod = o.PaymentMethod.ToString(),
                        CreatedDate = o.CreatedDate,
                        ItemCount = o.OrderItems.Count
                    })
                    .ToListAsync(cancellationToken);

            methodResult.Result = orders;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
