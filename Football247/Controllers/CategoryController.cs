using Football247.Application.Command.CategoryCmd;
using Football247.Application.Query.CategoryQuery;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models.Paging;
using Shared.Response;
using System.Net;

namespace Football247.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<CategoryDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<CategoryDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllCategoryQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        //[Authorize(Policy = Permissions.Categories.Create)]
        [ProducesResponseType(typeof(MethodResult<CategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<CategoryDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromQuery] CreateCategoryCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPut]
        [Route("update")]
        //[Authorize(Policy = Permissions.Categories.Edit)]
        public async Task<IActionResult> Update([FromQuery] UpdateCategoryCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpDelete]
        [Route("delete")]
        //[Authorize(Policy = Permissions.Categories.Delete)]
        public async Task<IActionResult> Delete([FromQuery] DeleteCategoryCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}
