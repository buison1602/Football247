using Microsoft.AspNetCore.Identity;

namespace Football247.Repositories.IRepository
{
    public interface ITokenRepository
    {
        string CreateJWTToken(IdentityUser user, List<string> roles);
    }
}
