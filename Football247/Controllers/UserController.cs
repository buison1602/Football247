using Football247.Application.Command.UserCmd;
using Football247.Application.Query.Role;
using Football247.Application.Query.UserQuery;
using Football247.Domain.Models.CommandModels.UserCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Role;
using Football247.Domain.Models.EntityModels.DTOs.User;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Shared.Common.Models.Paging;
using Shared.Response;
using System.Net;

namespace Football247.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("get-all")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<UserDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetAllUsers([FromQuery] GetAllUsersQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("role/{roleName}")]
        [ProducesResponseType(typeof(MethodResult<PagingItemsModel<UserDto>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUsersByRole([FromQuery] GetUsersByRoleQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpGet("id")]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetUserById([FromQuery] GetUserByIdQuery request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPost]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid id, [FromBody] UpdateUserCommand request)
        {
            request.Id = id;
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }


        [HttpDelete("id")]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<bool>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteUser([FromBody] DeleteUserCommand request)
        {
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }

        /// <summary>
        /// Admin endpoint để cập nhật thông tin user chi tiết
        /// Bao gồm: vai trò, quyền hạn, security settings, points, spins, etc.
        /// </summary>
        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(MethodResult<UserDto>), (int)HttpStatusCode.Unauthorized)]
        public async Task<IActionResult> UpdateUserAsAdmin([FromRoute] Guid id, [FromBody] UpdateUserAdminCommand request)
        {
            request.Id = id;
            var queryResult = await _mediator.Send(request).ConfigureAwait(false);
            return queryResult.GetActionResult();
        }
    }
}
