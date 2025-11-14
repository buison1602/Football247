using AutoMapper;
using Football247.Models.DTOs.User;
using Football247.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Football247.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IMapper mapper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _mapper = mapper;
        }


        public async Task<(IdentityResult result, UserDto? userDto)> CreateUserAsync(CreateUserDto createUserDto)
        {
            var existingUser = await _userManager.FindByEmailAsync(createUserDto.Email);
            //Console.WriteLine("/n/n------------" + existingUser + "-------------/n/n");
            if (existingUser != null)
            {
                return (IdentityResult.Failed(new IdentityError { 
                    Code = "DuplicateEmail", 
                    Description = $"Email {createUserDto.Email} is already in use." 
                }), null);
            }

            var newUser = new ApplicationUser
            {
                UserName = createUserDto.Email,
                Email = createUserDto.Email,
            };

            var result = await _userManager.CreateAsync(newUser, createUserDto.Password);

            if (!result.Succeeded)
            {
                return (result, null); 
            }

            if (await _roleManager.RoleExistsAsync(createUserDto.Role))
            {
                await _userManager.AddToRoleAsync(newUser, createUserDto.Role);
            }

            var userDto = _mapper.Map<UserDto>(newUser);
            return (result, userDto);
        }


        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _userManager.Users.ToListAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }


        public async Task<UserDto?> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto?>(user);
        }


        public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(string roleName)
        {
            var roleExists = await _roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                throw new ArgumentException($"Role '{roleName}' does not exist.");
            }

            var users = await _userManager.GetUsersInRoleAsync(roleName);
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }


        public async Task<IdentityResult> DeleteUserAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { 
                    Code = "NotFound", 
                    Description = "User not found." });
            }

            var result = await _userManager.DeleteAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to delete user {user.UserName}");
            }

            return result;
        }


        public async Task<IdentityResult> UpdateUserAsync(string id, UpdateUserDto updateUserDto)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { 
                    Code = "NotFound", 
                    Description = "User not found." });
            }

            var existingUser = await _userManager.FindByNameAsync(updateUserDto.Name);

            if (existingUser != null && existingUser.Id != id)
            {
                return IdentityResult.Failed(new IdentityError
                {
                    Code = "DuplicateUserName",
                    Description = $"Username '{updateUserDto.Name}' is already taken."
                });
            }

            user.UserName = updateUserDto.Name;
            user.AvatarUrl = updateUserDto.AvatarUrl;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Failed to update user  {user.UserName}");
            }

            return result;
        }
    }
}
