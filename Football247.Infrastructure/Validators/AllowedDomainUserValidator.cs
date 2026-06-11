using Microsoft.AspNetCore.Identity;

namespace Football247.IdentityExtensions
{
    public class AllowedDomainUserValidator<TUser> : IUserValidator<TUser> where TUser : class
    {
        private readonly string _allowedDomain = "gmail.com";

        public Task<IdentityResult> ValidateAsync(UserManager<TUser> manager, TUser user)
        {
            var email = manager.GetEmailAsync(user).Result;

            if (email != null && email.EndsWith($"@{_allowedDomain}", StringComparison.OrdinalIgnoreCase))
            {
                return Task.FromResult(IdentityResult.Success);
            }
            else
            {
                return Task.FromResult(IdentityResult.Failed(new IdentityError
                {
                    Code = "InvalidDomain",
                    Description = $"Only allow registration with domain name @{_allowedDomain}."
                }));
            }
        }
    }
}
