using Football247.Application.Query.DashboardQuery;
using Football247.Application.Query.DashboardQuery;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Football247.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AnalyticsController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("TEST-CD-haha-hehe")]
        public async Task<IActionResult> GetRevenue_test()
        => (await _mediator.Send(new GetRevenueQuery())).GetActionResult();

        // GET /api/analytics/top-categories?period=1|2
        [HttpGet("top-categories")]
        public async Task<IActionResult> GetTopCategories([FromQuery] GetTopCategoriesQuery query)
            => (await _mediator.Send(query)).GetActionResult();

        [HttpGet("revenue")]
        public async Task<IActionResult> GetRevenue()
        => (await _mediator.Send(new GetRevenueQuery())).GetActionResult();

        [HttpGet("top-products")]
        public async Task<IActionResult> GetTopProducts()
            => (await _mediator.Send(new GetTopProductsQuery())).GetActionResult();
    }
}
