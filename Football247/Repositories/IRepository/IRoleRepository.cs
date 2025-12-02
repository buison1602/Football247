using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Football247.Repositories.IRepository
{
    public interface IRoleRepository 
    {
        Task<List<IdentityRole>> GetAllAsync();
        Task<IdentityRole?> GetByIdAsync(string id);
        Task<IdentityResult> CreateAsync(IdentityRole role);
        Task<IdentityResult> UpdateAsync(IdentityRole role);
        Task<IdentityResult> DeleteAsync(IdentityRole role);


        // Permissions Management 
        Task<IList<Claim>> GetClaimsAsync(IdentityRole role);
        Task AddClaimAsync(IdentityRole role, Claim claim);
        Task RemoveClaimAsync(IdentityRole role, Claim claim);
    }
}
