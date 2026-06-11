using Football247.Application.Command.Store.CartCmd;
using Football247.Application.Query.StoreQuery.CartQuery;
using Football247.Domain.Models.EntityModels.DTOs.Cart;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CartController(IMediator mediator)
        {
            _mediator = mediator;
        }


        #region Command 
        [HttpPost]
        //[Authorize(Policy = Permissions.Cart.Create)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromQuery] AddCartItemCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        [HttpPut("remove-item")]
        //[Authorize(Policy = Permissions.Cart.Update)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Remove([FromQuery] RemoveCartItemCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        [HttpPut("update-quantity")]
        //[Authorize(Policy = Permissions.Cart.Update)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateQuantity([FromQuery] UpdateCartItemQuantityCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
        #endregion


        #region Query
        [HttpGet("userId/{userId}")]
        //[Authorize(Policy = Permissions.Cart.View)]
        [ProducesResponseType(typeof(MethodResult<CartDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<CartDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCartByUserId([FromRoute] Guid userId)
        {
            var queryResult = await _mediator.Send(new GetCartByUserIdQuery { UserId = userId }).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        [HttpGet("get-total-item")]
        //[Authorize(Policy = Permissions.Cart.View)]
        [ProducesResponseType(typeof(MethodResult<int>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<int>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetTotalItem([FromQuery] GetCartItemCountQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
        #endregion
    }
}
