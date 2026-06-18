using Football247.Domain.Models.EntityModels.DTOs.Dashboard;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;

namespace Football247.Application.Query.DashboardQuery
{
    public class GetTopProductsQuery : IRequest<MethodResult<List<TopProductDto>>>
    {
        public TimePeriod Period { get; set; } = TimePeriod.Month;
    }

    public class GetTopProductsQueryHandler : IRequestHandler<GetTopProductsQuery, MethodResult<List<TopProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTopProductsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<TopProductDto>>> Handle(GetTopProductsQuery request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<List<TopProductDto>>();
            var now = DateTime.UtcNow;

            var result = await _unitOfWork.OrderRepository.ReadQueryable
                .Where(o => o.PaymentStatus == EnumPaymentStatus.Paid)
                .Include(o => o.OrderItems)
                .SelectMany(o => o.OrderItems)
                .GroupBy(i => new { i.ProductName, i.ProductThumbnail })
                .Select(g => new TopProductDto
                {
                    ProductId = g.First().ProductId,
                    ProductName = g.Key.ProductName,
                    ThumbnailUrl = g.Key.ProductThumbnail,
                    TotalSold = g.Sum(i => i.Quantity),
                    TotalRevenue = g.Sum(i => i.Price * i.Quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .ToListAsync(cancellationToken);

            methodResult.Result = result;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
