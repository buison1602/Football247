using Football247.Application.Command.TeamCmd;
using Football247.Application.Command.TeamCmd;
using Football247.Application.Query.TeamQuery;
using Football247.Domain.Models.EntityModels.DTOs.Team;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Collections.Generic;
using System.Net;

namespace Football247.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TeamController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<List<TeamDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<TeamDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var queryResult = await _mediator.Send(new GetAllTeamQuery()).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        //[Authorize(Policy = Permissions.Categories.Create)]
        [ProducesResponseType(typeof(MethodResult<TeamDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<TeamDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromQuery] CreateTeamCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPut]
        [Route("update")]
        //[Authorize(Policy = Permissions.Categories.Edit)]
        [ProducesResponseType(typeof(MethodResult<TeamDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<TeamDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update([FromQuery] UpdateTeamCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpDelete]
        [Route("delete")]
        //[Authorize(Policy = Permissions.Categories.Delete)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromQuery] DeleteTeamCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}
