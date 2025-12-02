using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Football247.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleRepository(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<IdentityRole>> GetAllAsync()
        {
            return await _roleManager.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IdentityRole?> GetByIdAsync(string id)
        {
            return await _roleManager.FindByIdAsync(id);
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole role)
        {
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole role)
        {
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole role)
        {
            return await _roleManager.DeleteAsync(role);
        }


        // QUẢN LÝ CLAIM / PERMISSION
        public async Task<IList<Claim>> GetClaimsAsync(IdentityRole role)
        {
            return await _roleManager.GetClaimsAsync(role);
        }

        public async Task AddClaimAsync(IdentityRole role, Claim claim)
        {
            await _roleManager.AddClaimAsync(role, claim);
        }

        public async Task RemoveClaimAsync(IdentityRole role, Claim claim)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }
    }
}
