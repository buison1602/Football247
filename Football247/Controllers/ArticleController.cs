using Football247.Application.Command.ArticleCmd;
using Football247.Application.Query.ArticleQuery;
using Football247.Authorization;
using Football247.Domain.Models.EntityModels.DTOs.Article;
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
    //[Authorize(Roles = "Admin")]

    public class ArticleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ArticleController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<ArticleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllArticlesQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        [HttpGet("priority")]
        [ProducesResponseType(typeof(MethodResult<List<ArticlesDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<ArticlesDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArticlesByPriority([FromQuery] GetArticlesByPriority request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        [Authorize(Policy = Permissions.Articles.Create)]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromBody] CreateArticleCommand command)
        {
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }



        [HttpPut]
        [Route("{id:guid}")]
        [Authorize(Policy = Permissions.Articles.Edit)]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleCommand command)
        {
            command.Id = id;
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }


        [HttpDelete]
        [Route("{id:guid}")]
        [Authorize(Policy = Permissions.Articles.Delete)]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            DeleteArticleCommand command = new DeleteArticleCommand();
            command.Id = id;
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }


        [HttpGet("id")]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArticleById([FromQuery] GetArticleByIdQuery query)
        {
            var queryResult = await _mediator.Send(query).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet]
        [Route("articleSlug")]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ArticleDto), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArticleBySlug([FromQuery] GetArticleBySlugQuery query)
        {
            var queryResult = await _mediator.Send(query).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("category")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<ArticleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArticleByCategory([FromQuery] GetArticleByCategoryQuery query)
        {
            var queryResult = await _mediator.Send(query).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("team")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<ArticleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArticleByTeam([FromQuery] GetArticleByTeamQuery query)
        {
            var queryResult = await _mediator.Send(query).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("tag")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<ArticleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<ArticleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetArticlesByTag([FromQuery] GetArticleByTagQuery query)
        {
            var queryResult = await _mediator.Send(query).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost("{id}/approve")]
        [Authorize(Policy = Permissions.Articles.Create)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RequestArticleApproval(Guid id)
        {
            var command = new RequestArticleApprovalCommand { ArticleId = id };
            var commandResult = await _mediator.Send(command).ConfigureAwait(false);
            return commandResult.GetActionResult();
        }

        [HttpGet("pending")]
        [Authorize(Policy = Permissions.Articles.Approve)]
        [ProducesResponseType(typeof(MethodResult<List<ArticleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<ArticleDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPendingArticles()
        {
            var query = new GetArticlePendingQuery();
            var queryResult = await _mediator.Send(query).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("latest")]
        [ProducesResponseType(typeof(MethodResult<List<ArticlesDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<ArticlesDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetLatestArticles()
        {
            var query = new GetLatestArticleQuery();
            var queryResult = await _mediator.Send(query).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}
