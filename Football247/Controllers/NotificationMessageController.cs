using Football247.Application.Query.ArticleQuery;
using Football247.Application.Query.NotificationMessageQuery;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Notification;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models.Paging;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class NotificationMessageController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationMessageController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<NotificationMessageDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<NotificationMessageDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] GetNotificationMessageByUserIdQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}
