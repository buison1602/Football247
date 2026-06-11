using Football247.Application.Command.CategoryCmd;
using Football247.Application.Command.Store.ProductCategoryCmd;
using Football247.Application.Query.StoreQuery.ProductCategoryQuery;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductCategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductCategoryController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Command 

        [HttpPost]
        //[Authorize(Policy = Permissions.ProductCategories.Create)]
        [ProducesResponseType(typeof(MethodResult<ProductCategoryDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<ProductCategoryDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromQuery] CreateProductCategoryCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        [HttpPut]
        [Route("update")]
        //[Authorize(Policy = Permissions.ProductCategories.Edit)]
        public async Task<IActionResult> Update([FromQuery] UpdateProductCategoryCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpDelete]
        [Route("delete")]
        //[Authorize(Policy = Permissions.ProductCategories.Delete)]
        public async Task<IActionResult> Delete([FromQuery] DeleteProductCategoryCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        #endregion


        #region Query

        [HttpGet]
        [ProducesResponseType(typeof(MethodResult<List<ProductCategoryDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<ProductCategoryDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var queryResult = await _mediator.Send(new GetAllProductCategoryQuery()).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        #endregion

    }
}
