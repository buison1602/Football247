using Football247.Application.Command.CommentCmd;
using Football247.Application.Query.CommentQuery;
using Football247.Domain.Models.EntityModels.DTOs.Comment;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;

namespace Football247.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CommentController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CommentController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("articleId")]
        [ProducesResponseType(typeof(MethodResult<List<CommentDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<CommentDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetCommentsByArticleId([FromQuery] Guid articleId)
        {
            var result = await _mediator.Send(new GetCommentsByArticleQuery { ArticleId = articleId });
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        [ProducesResponseType(typeof(MethodResult<CommentDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(CommentDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> PostComment([FromBody] CreateCommentCommand command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        [HttpPost("report")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ReportComment([FromBody] ReportCommentCommand command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteComment([FromRoute] Guid commentId)
        {
            var commandResult = await _mediator.Send(new DeleteCommentCommand { CommentId = commentId }).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        [Authorize]
        [HttpGet("get-comment-reported")]
        [ProducesResponseType(typeof(MethodResult<List<CommentDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<CommentDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetReportedComments()
        {
            var commandResult = await _mediator.Send(new GetCommentReportedQuery()).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }
    }
}
