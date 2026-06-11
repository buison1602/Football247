using Football247.Application.Command.SendEmailCmd;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SendEmailController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SendEmailController(IMediator mediator)
        {
            _mediator = mediator;
        }


        /// <summary>
        /// SendMail
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> SendEmail([FromBody] SendEmailCommand command)
        {
            MethodResult<bool> commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}
