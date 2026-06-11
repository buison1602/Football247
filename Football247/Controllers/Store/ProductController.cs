using Football247.Application.Command.Store.ProductCategoryCmd;
using Football247.Application.Command.Store.ProductCmd;
using Football247.Application.Query.StoreQuery.ProductCategoryQuery;
using Football247.Application.Query.StoreQuery.ProductQuery;
using Football247.Domain.Models.EntityModels.DTOs.Product;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models.Paging;
using Shared.Response;
using System.Net;

namespace Football247.Api.Controllers.Store
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        #region Command 

        [HttpPost]
        //[Authorize(Policy = Permissions.Products.Create)]
        [ProducesResponseType(typeof(MethodResult<ProductDetailDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<ProductDetailDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromQuery] CreateProductCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        [HttpPut]
        [Route("update")]
        //[Authorize(Policy = Permissions.Products.Edit)]
        public async Task<IActionResult> Update([FromQuery] UpdateProductCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpDelete]
        [Route("delete")]
        //[Authorize(Policy = Permissions.Products.Delete)]
        public async Task<IActionResult> Delete([FromQuery] DeleteProductCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        [HttpPut("toggle-active")]
        //[Authorize(Policy = Permissions.Products.Edit)]
        public async Task<IActionResult> ToggleActive([FromBody] UpdateToggleActiveProduct request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        #endregion


        #region Query

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(MethodResult<List<ProductDetailDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<List<ProductDetailDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll()
        {
            var queryResult = await _mediator.Send(new GetAllProductQuery()).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("get-by-id")]
        [ProducesResponseType(typeof(MethodResult<ProductDetailDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<ProductDetailDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetById([FromQuery] GetProductById request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        //[HttpGet("get-by-category-id")]
        //[ProducesResponseType(typeof(MethodResult<List<ProductDetailDto>>), (int)HttpStatusCode.OK)]
        //[ProducesResponseType(typeof(MethodResult<List<ProductDetailDto>>), (int)HttpStatusCode.InternalServerError)]
        //public async Task<IActionResult> GetByCategoryId([FromQuery] GetProductsByProductCategoryIdQuery request)
        //{
        //    var queryResult = await _mediator.Send(request).ConfigureAwait(false);
        //    return queryResult.GetActionResult();
        //}

        [HttpGet("search")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<ProductDetailDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<ProductDetailDto>>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Search([FromQuery] SearchProductQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        #endregion

    }
}
