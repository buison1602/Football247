using Football247.Application.Query.StandingQuery;
using Football247.Domain.Models.EntityModels.DTOs.Standing;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StandingController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StandingController(IMediator mediator)   
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<List<StandingDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<StandingDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var request = new GetAllStandingQuery();
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}
