using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Football247.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<IdentityRole<Guid>> _roleManager;

        public RoleRepository(RoleManager<IdentityRole<Guid>> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<List<IdentityRole<Guid>>> GetAllAsync()
        {
            return await _roleManager.Roles
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IdentityRole<Guid>?> GetByIdAsync(Guid id)
        {
            return await _roleManager.FindByIdAsync(id.ToString());
        }

        public async Task<IdentityResult> CreateAsync(IdentityRole<Guid> role)
        {
            return await _roleManager.CreateAsync(role);
        }

        public async Task<IdentityResult> UpdateAsync(IdentityRole<Guid> role)
        {
            return await _roleManager.UpdateAsync(role);
        }

        public async Task<IdentityResult> DeleteAsync(IdentityRole<Guid> role)
        {
            return await _roleManager.DeleteAsync(role);
        }


        // QUẢN LÝ CLAIM / PERMISSION
        public async Task<IList<Claim>> GetClaimsAsync(IdentityRole<Guid> role)
        {
            return await _roleManager.GetClaimsAsync(role);
        }

        public async Task AddClaimAsync(IdentityRole<Guid> role, Claim claim)
        {
            await _roleManager.AddClaimAsync(role, claim);
        }

        public async Task RemoveClaimAsync(IdentityRole<Guid> role, Claim claim)
        {
            await _roleManager.RemoveClaimAsync(role, claim);
        }
    }
}
