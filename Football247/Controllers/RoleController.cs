using Football247.Data; // Để dùng Roles.Admin
using Football247.Models.DTOs.Role;
using Football247.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = Roles.Admin)] 
    public class RolesController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RolesController> _logger;

        public RolesController(IRoleService roleService, ILogger<RolesController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var roles = await _roleService.GetAllRolesWithPermissionsAsync();
            return Ok(roles);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var role = await _roleService.GetRoleByIdAsync(id);
            if (role == null)
            {
                return NotFound($"Không tìm thấy Role với ID: {id}");
            }
            return Ok(role);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrUpdateRoleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _roleService.CreateRoleAsync(dto);
                if (!result)
                {
                    return BadRequest($"Role với tên {dto.Name} đã tồn tại");
                }

                return Ok("Thêm mới Role thành công.");
            }
            catch (ArgumentException ex) 
            {
                return BadRequest(ex.Message); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating role");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] CreateOrUpdateRoleDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var result = await _roleService.UpdateRoleAsync(id, dto);
                if (!result)
                {
                    return NotFound($"Không tìm thấy role với ID {id} hoặc lỗi cập nhật");
                }

                return Ok("Cập nhật Role thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating role {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var result = await _roleService.DeleteRoleAsync(id);
                if (!result)
                {
                    return BadRequest("Xóa Role thất bại");
                }

                return Ok("Xóa Role thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting role {id}");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("permissions-source")]
        public IActionResult GetAllSystemPermissions()
        {
            var permissions = _roleService.GetAllSystemPermissions();
            return Ok(permissions);
        }
    }
}