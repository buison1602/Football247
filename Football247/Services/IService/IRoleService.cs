using Football247.Models.DTOs.Role;

namespace Football247.Services.IService
{
    public interface IRoleService
    {
        Task<List<RoleDto>> GetAllRolesWithPermissionsAsync();
        Task<RoleDto?> GetRoleByIdAsync(string id);
        Task<bool> CreateRoleAsync(CreateOrUpdateRoleDto dto);
        Task<bool> UpdateRoleAsync(string id, CreateOrUpdateRoleDto dto);
        Task<bool> DeleteRoleAsync(string id);


        // Lấy tất cả các hằng số Permission từ Code
        List<string> GetAllSystemPermissions();
    }
}
