using Football247.Application.Command.Store.OrderCmd;
using Football247.Application.Query.StoreQuery.OrderQuery;
using Football247.Domain.Models.EntityModels.DTOs.Order;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;
using System.Security.Claims;

namespace Football247.Api.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrderController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // POST /api/orders/checkout
        [HttpPost("checkout")]
        [Authorize]
        [ProducesResponseType(typeof(MethodResult<string>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutCommand request)
        {
            // Lấy UserId từ JWT token
            request.UserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(request);
            return result.GetActionResult();
        }

        // GET /api/orders/my-orders
        [HttpGet("my-orders")]
        [Authorize]
        [ProducesResponseType(typeof(MethodResult<List<OrderSummaryDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMyOrders([FromQuery] bool? paidOnly = null)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new GetMyOrdersQuery { UserId = userId, PaidOnly = paidOnly });
            return result.GetActionResult();
        }

        // GET /api/orders/{id}
        [HttpGet("{id:guid}")]
        [Authorize]
        [ProducesResponseType(typeof(MethodResult<OrderDetailDto>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrderDetail(Guid id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var result = await _mediator.Send(new GetOrderDetailQuery { OrderId = id, UserId = userId });
            return result.GetActionResult();
        }

        // GET /api/orders/admin?page=1&pageSize=20&statusFilter=Pending
        [HttpGet("admin")]
        //[Authorize(Policy = Permissions.Order.Read)]
        [ProducesResponseType(typeof(MethodResult<List<OrderSummaryDto>>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetOrdersAdmin([FromQuery] GetOrdersAdminQuery query)
        {
            var result = await _mediator.Send(query);
            return result.GetActionResult();
        }

        // PATCH /api/Order/{id}/status
        [HttpPatch("{id:guid}/status")]
        //[Authorize(Policy = Permissions.Order.Update)]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateOrderStatusCommand command)
        {
            command.OrderId = id;
            var result = await _mediator.Send(command);
            return result.GetActionResult();
        }
    }
}
