using Football247.Application.Command.TagCmd;
using Football247.Application.Query.TagQuery;
using Football247.Authorization;
using Football247.Domain.Models.EntityModels.DTOs.Tag;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models.Paging;
using Shared.Response;
using System.Net;

namespace Football247.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<TagDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<TagDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllTagQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        //[Authorize(Policy = Permissions.Tags.Create)]
        [ProducesResponseType(typeof(MethodResult<TagDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<TagDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromQuery] CreateTagCommand request)
        {
            var commandResult = await _mediator.Send(request).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }


        [HttpPut]
        [Route("update")]
        //[Authorize(Policy = Permissions.Tags.Edit)]
        [ProducesResponseType(typeof(MethodResult<TagDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<TagDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update([FromQuery] UpdateTagCommand request)
        {
            var commandResult = await _mediator.Send(request).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }


        [HttpDelete]
        [Route("delete")]
        //[Authorize(Policy = Permissions.Tags.Delete)]
        [ProducesResponseType(typeof(MethodResult<TagDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<TagDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromQuery] DeleteTagCommand request)
        {
            var commandResult = await _mediator.Send(request).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}
