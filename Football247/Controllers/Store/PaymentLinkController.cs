using Football247.Application.Command.PaymentCmd;
using Football247.Application.Command.ProcessWebhookCmd;
using Football247.Application.Command.Store.CartCmd;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayOS.Models.Webhooks;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentLinkController : ControllerBase
    {
        private readonly IMediator _mediator;
        public PaymentLinkController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        //[Authorize(Policy = Permissions.Cart.Create)]
        [ProducesResponseType(typeof(MethodResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreatePaymentLinkCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        // Endpoint: POST /api/PaymentLink/payos
        [HttpPost("payos")]
        public async Task<IActionResult> PayOSWebhook([FromBody] Webhook webhookData)
        {
            try
            {
                var command = new ProcessWebhookCommand
                {
                    WebhookData = webhookData
                };

                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    // Trả về 200 OK để PayOS biết đã xử lý thành công
                    return Ok(new { success = true });
                }

                return BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }
    }
}
