using Football247.Domain.Models.EntityModels.DTOs.Dashboard;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace Football247.Application.Query.DashboardQuery
{
    public class GetTopCategoriesQuery : IRequest<MethodResult<List<TopCategoryDto>>>
    {
        public TimePeriod Period { get; set; } = TimePeriod.Month;
    }

    public class GetTopCategoriesQueryHandler : IRequestHandler<GetTopCategoriesQuery, MethodResult<List<TopCategoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetTopCategoriesQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<TopCategoryDto>>> Handle(GetTopCategoriesQuery request, CancellationToken cancellationToken)
        {
            var methodResult = new MethodResult<List<TopCategoryDto>>();
            var now = DateTime.UtcNow;
            var fromDate = request.Period == TimePeriod.Week ? now.AddDays(-7) : now.AddDays(-30);

            var result = await _unitOfWork.ArticleRepository.ReadQueryable
                .Include(a => a.Category)
                //.Where(a => a.IsApproved && a.CreatedDate >= fromDate)
                .GroupBy(a => new { a.Category.Id, a.Category.Name })
                .Select(g => new TopCategoryDto
                {
                    CategoryName = g.Key.Name,
                    TotalViews = g.Sum(a => a.ViewCount),
                    ArticleCount = g.Count()
                })
                .OrderByDescending(x => x.TotalViews)
                .ToListAsync(cancellationToken);

            methodResult.Result = result;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
