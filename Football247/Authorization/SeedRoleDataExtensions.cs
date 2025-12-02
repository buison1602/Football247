using Football247.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore; // Để dùng AnyAsync nếu cần
using System.Security.Claims;

namespace Football247.Authorization
{
    public static class SeedDataExtensions
    {
        public static async Task SeedRolesAndPermissions(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // 1. SEED ADMIN ROLE (Full Quyền)
            var adminRole = await roleManager.FindByNameAsync(Roles.Admin);
            if (adminRole == null)
            {
                adminRole = new IdentityRole(Roles.Admin);
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

            // 2. SEED MEMBER ROLE (Quyền hạn chế)
            var memberRole = await roleManager.FindByNameAsync(Roles.Member);
            if (memberRole == null)
            {
                memberRole = new IdentityRole(Roles.Member);
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
        }

        private static async Task AddPermissionIfMissing(RoleManager<IdentityRole> roleManager, IdentityRole role, string permission)
        {
            var claims = await roleManager.GetClaimsAsync(role);
            if (!claims.Any(c => c.Type == CustomClaimTypes.Permission && c.Value == permission))
            {
                await roleManager.AddClaimAsync(role, new Claim(CustomClaimTypes.Permission, permission));
            }
        }
    }
}