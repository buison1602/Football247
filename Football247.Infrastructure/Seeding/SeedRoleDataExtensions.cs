using Football247.Application.Common.Data; 
using Football247.Models.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;

namespace Football247.Authorization
{
    public static class SeedDataExtensions
    {
        public static async Task SeedRolesAndPermissions(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();

            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();

            // Admin
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole == null)
            {
                adminRole = new IdentityRole<Guid>(Roles.Admin);
                await roleManager.CreateAsync(adminRole);
            }

            var allPermissions = Permissions.GetAllPermissions();
            var adminCurrentClaims = await roleManager.GetClaimsAsync(adminRole);

            foreach (var permission in allPermissions)
            {
                if (!adminCurrentClaims.Any(c => c.Type == CustomClaimTypes.Permission && c.Value == permission))
                {
                    await roleManager.AddClaimAsync(adminRole, new Claim(CustomClaimTypes.Permission, permission));
                }
            }

            // Member
            var memberRole = await roleManager.FindByNameAsync(Roles.Member);
            if (memberRole == null)
            {
                memberRole = new IdentityRole<Guid>(Roles.Member);
                await roleManager.CreateAsync(memberRole);
            }

            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Articles.View);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Articles.Create);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Articles.Edit);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Articles.Delete);

            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Categories.View);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Categories.Create);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Categories.Edit);

            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Tags.View);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Tags.Create);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Tags.Edit);

            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Comments.View);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Comments.Create);
            await AddPermissionIfMissing(roleManager, memberRole, Permissions.Comments.Delete);

            // User
            var userRole = await roleManager.FindByNameAsync(Roles.User);
            if (userRole == null)
            {
                userRole = new IdentityRole<Guid>(Roles.User);
                await roleManager.CreateAsync(userRole);

                await AddPermissionIfMissing(roleManager, userRole, Permissions.Articles.View);
                await AddPermissionIfMissing(roleManager, userRole, Permissions.Comments.Create);
                await AddPermissionIfMissing(roleManager, userRole, Permissions.Comments.Delete);
            }
        }

        private static async Task AddPermissionIfMissing(RoleManager<IdentityRole<Guid>> roleManager, IdentityRole<Guid> role, string permission)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            if (!claims.Any(c => c.Type == CustomClaimTypes.Permission && c.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
            }
        }

        public static async Task SeedAdminUser(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            const string adminEmail = "superadmin@gmail.com";
            const string adminPassword = "Dhtl@123";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    CreatedDate = DateTime.UtcNow
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                    throw new Exception($"Seed admin user thất bại: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(adminUser, Roles.Admin))
            {
                await userManager.AddToRoleAsync(adminUser, Roles.Admin);
            }
        }
    }
}