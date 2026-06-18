using Football247.Domain.Models.EntityModels.DTOs.Dashboard;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;

namespace Football247.Application.Query.DashboardQuery
{
    public class GetRevenueQuery : IRequest<MethodResult<List<RevenueDataDto>>>
    {
        public TimePeriod Period { get; set; } = TimePeriod.Day;
    }

    public class GetRevenueQueryHandler : IRequestHandler<GetRevenueQuery, MethodResult<List<RevenueDataDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetRevenueQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<RevenueDataDto>>> Handle(GetRevenueQuery request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<List<RevenueDataDto>>();
            var now = DateTime.UtcNow;

            var orders = await _unitOfWork.OrderRepository.Queryable
                .Where(o => o.PaymentStatus == EnumPaymentStatus.Paid)// &&o.CreatedDate.Year == now.Year)  // do là đồ án nên chỉ có trong năm nay thôi 
                .ToListAsync(cancellationToken);

            // Tạo đủ 12 tháng, tháng nào không có đơn thì = 0
            var result = Enumerable.Range(1, 12)
                .Select(month => new RevenueDataDto
                {
                    Label = $"T{month}",
                    Amount = orders
                        .Where(o => o.CreatedDate.Month == month)
                        .Sum(o => o.TotalAmount)
                })
                .ToList();

            methodResult.Result = result;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
