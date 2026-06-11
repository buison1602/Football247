using Football247.Application.Command.RoleCmd;
using Football247.Application.Query.Role;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Role;
using Football247.Services.IService;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models.Paging;
using Shared.Response;
using System.Net;

namespace Football247.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(Roles = Roles.Admin)] 
    public class RolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RolesController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("get-all")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<RoleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<RoleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAll([FromQuery] GetAllRolesWithPermissionsQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("get-by-id")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<RoleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<RoleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetById([FromQuery] GetRoleByIdQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<RoleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<RoleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Create([FromQuery] CreateRoleCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPut("update")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<RoleDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<RoleDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Update([FromQuery] UpdateRoleCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpDelete("delete")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<bool>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Delete([FromQuery] DeleteRoleCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
               

        [HttpGet("permissions-source")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<bool>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllSystemPermissions([FromQuery] GetAllSystemPermissionsQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}