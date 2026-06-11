using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Football247.Repositories.IRepository
{
    public interface IRoleRepository 
    {
        Task<List<IdentityRole<Guid>>> GetAllAsync();
        Task<IdentityRole<Guid>?> GetByIdAsync(Guid id);
        Task<IdentityResult> CreateAsync(IdentityRole<Guid> role);
        Task<IdentityResult> UpdateAsync(IdentityRole<Guid> role);
        Task<IdentityResult> DeleteAsync(IdentityRole<Guid> role);


        // Permissions Management 
        Task<IList<Claim>> GetClaimsAsync(IdentityRole<Guid> role);
        Task AddClaimAsync(IdentityRole<Guid> role, Claim claim);
        Task RemoveClaimAsync(IdentityRole<Guid> role, Claim claim);
    }
}
