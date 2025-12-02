using AutoMapper;
using Football247.Authorization;
using Football247.Data;
using Football247.Models.DTOs.Role;
using Football247.Repositories.IRepository;
using Football247.Services.Caching;
using Football247.Services.IService;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Football247.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCacheService;
        private const string CacheKey = "roles";

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
        }


        public async Task<List<RoleDto>> GetAllRolesWithPermissionsAsync()
        {
            var cachedRoles = await _redisCacheService.GetDataAsync<List<RoleDto>>(CacheKey);
            if (cachedRoles != null)
            {
                return cachedRoles;
            }

            var roles = await _unitOfWork.RoleRepository.GetAllAsync();
            var roleDtos = new List<RoleDto>();

            foreach (var role in roles)
            {
                var claims = await _unitOfWork.RoleRepository.GetClaimsAsync(role);
                roleDtos.Add(new RoleDto
                {
                    Id = role.Id,
                    Name = role.Name,
                    Permissions = claims
                        .Where(c => c.Type == CustomClaimTypes.Permission)
                        .Select(c => c.Value)
                        .ToList()
                });
            }

            await _redisCacheService.SetDataAsync(CacheKey, roleDtos);

            return roleDtos;
        }


        public async Task<RoleDto?> GetRoleByIdAsync(string id)
        {
            string cacheKeyItem = $"{CacheKey}_{id}";

            var cachedRole = await _redisCacheService.GetDataAsync<RoleDto>(cacheKeyItem);
            if (cachedRole != null) return cachedRole;

            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            if (role == null) return null;

            var claims = await _unitOfWork.RoleRepository.GetClaimsAsync(role);
            var roleDto = new RoleDto
            {
                Id = role.Id,
                Name = role.Name,
                Permissions = claims
                    .Where(c => c.Type == CustomClaimTypes.Permission)
                    .Select(c => c.Value)
                    .ToList()
            };

            await _redisCacheService.SetDataAsync(cacheKeyItem, roleDto);

            return roleDto;
        }


        public async Task<bool> CreateRoleAsync(CreateOrUpdateRoleDto createOrUpdateRoleDto)
        {
            if (createOrUpdateRoleDto.Permissions != null && createOrUpdateRoleDto.Permissions.Any())
            {
                var validSystemPermissions = Permissions.GetAllPermissions();

                var invalidPermissions = createOrUpdateRoleDto.Permissions.Except(validSystemPermissions).ToList();

                if (invalidPermissions.Any())
                {
                    throw new ArgumentException($"Các quyền sau không hợp lệ: {string.Join(", ", invalidPermissions)}");
                }
            }

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var role = new IdentityRole(createOrUpdateRoleDto.Name);
                var result = await _unitOfWork.RoleRepository.CreateAsync(role);

                if (!result.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                if (createOrUpdateRoleDto.Permissions != null && createOrUpdateRoleDto.Permissions.Any())
                {
                    var validSystemPermissions = Permissions.GetAllPermissions();
                    foreach (var permissionName in createOrUpdateRoleDto.Permissions)
                    {
                        if (validSystemPermissions.Contains(permissionName))
                        {
                            await _unitOfWork.RoleRepository.AddClaimAsync(role, 
                                new Claim(CustomClaimTypes.Permission, permissionName));
                        }
                    }
                }

                await _unitOfWork.CommitTransactionAsync();
                await _redisCacheService.RemoveDataAsync(CacheKey);

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task<bool> UpdateRoleAsync(string id, CreateOrUpdateRoleDto createOrUpdateRoleDto)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
                if (role == null)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                role.Name = createOrUpdateRoleDto.Name;
                var updateResult = await _unitOfWork.RoleRepository.UpdateAsync(role);
                if (!updateResult.Succeeded)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    return false;
                }

                var currentClaims = await _unitOfWork.RoleRepository.GetClaimsAsync(role);
                var currentPermissions = currentClaims.Where(c => c.Type == CustomClaimTypes.Permission).ToList();

                foreach (var claim in currentPermissions)
                {
                    await _unitOfWork.RoleRepository.RemoveClaimAsync(role, claim);
                }

                var validSystemPermissions = Permissions.GetAllPermissions();
                foreach (var permissionName in createOrUpdateRoleDto.Permissions)
                {
                    if (validSystemPermissions.Contains(permissionName))
                    {
                        await _unitOfWork.RoleRepository.AddClaimAsync(role, 
                            new Claim(CustomClaimTypes.Permission, permissionName));
                    }
                }

                await _unitOfWork.CommitTransactionAsync();

                await _redisCacheService.RemoveDataAsync(CacheKey);
                await _redisCacheService.RemoveDataAsync($"{CacheKey}_{id}");

                return true;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;
            }
        }


        public async Task<bool> DeleteRoleAsync(string id)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            if (role == null) return false;

            if (role.Name == Roles.Admin) return false; 

            var result = await _unitOfWork.RoleRepository.DeleteAsync(role);

            if (result.Succeeded)
            {
                await _redisCacheService.RemoveDataAsync(CacheKey);
                await _redisCacheService.RemoveDataAsync($"{CacheKey}_{id}");
                return true;
            }
            return false;
        }


        public List<string> GetAllSystemPermissions()
        {
            return Permissions.GetAllPermissions();
        }
    }
}
