using Football247.Application.Command.WorkFlowCmd;
using Football247.Authorization;
using Football247.Services.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;

namespace Football247.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class WorkFlowController : ControllerBase
    {
        private readonly IMediator _mediator;

        public WorkFlowController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("approve")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ReviewArticle([FromBody] ReviewRequestArticleCommand command)
        {
            var commandResult = await _mediator.Send(command);
            return commandResult.GetActionResult();
        }
    }
}
