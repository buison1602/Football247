using Football247.Application.Command.AuthCmd;
using Football247.Domain.Models.CommandModels.AuthCommand;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Auth;
using Football247.Services.IService;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Shared.Response;
using System.Net;

namespace Football247.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }   


        [HttpPost]
        [Route("Register")]
        [ProducesResponseType(typeof(MethodResult<AuthResultDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<AuthResultDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var queryResult = await _mediator.Send(command).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        [Route("Login")]
        [ProducesResponseType(typeof(MethodResult<AuthResultDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<AuthResultDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var queryResult = await _mediator.Send(command).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        [Route("Refresh")]
        [Authorize]
        [ProducesResponseType(typeof(MethodResult<AuthResultDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<AuthResultDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Refresh([FromBody] RefeshTokenCommand command)
        {
            var queryResult = await _mediator.Send(command).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        [Route("Logout")]
        [Authorize]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Logout([FromBody] LogoutCommand command)
        {
            var queryResult = await _mediator.Send(command).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        [Route("ForgotPassword")]
        [ProducesResponseType(typeof(MethodResult<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var queryResult = await _mediator.Send(command).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPut]
        [Route("ChangePassword")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var queryResult = await _mediator.Send(command).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}

