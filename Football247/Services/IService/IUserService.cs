using Microsoft.AspNetCore.Identity;
using Football247.Models.DTOs.User;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(string id);
    Task<(IdentityResult result, UserDto? userDto)> CreateUserAsync(CreateUserDto createUserDto);
    Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto updateUserDto);
    Task<IdentityResult> DeleteUserAsync(string id);
    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName);
}