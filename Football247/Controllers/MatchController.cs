using Football247.Application.Query.Match;
using Football247.Application.Query.StandingQuery;
using Football247.Domain.Models.EntityModels.DTOs.Match;
using Football247.Domain.Models.EntityModels.DTOs.Standing;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MatchController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<List<MatchFixtureDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<MatchFixtureDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] bool isResult)
        {
            var request = new GetTodayMatchQuery() { isResult = isResult };
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("get-all")]
        [ProducesResponseType(typeof(MethodResult<List<MatchFixtureDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<MatchFixtureDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] bool isResult)
        {
            var request = new GetAllMatchQuery() { isResult = isResult };
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}
