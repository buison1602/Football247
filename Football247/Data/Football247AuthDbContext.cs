using Football247.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Football247.Data
{
    public class Football247AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public Football247AuthDbContext(DbContextOptions<Football247AuthDbContext> dbContextOptions) : base(dbContextOptions)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Seed data for roles
            var adminRoleId = "81470c42-0690-41b4-8b44-d6e388086964";
            var userRoleId = "79620ca9-0980-410b-96ad-04e05a20e80e";

            var roles = new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = adminRoleId,
                    ConcurrencyStamp = adminRoleId,
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id = userRoleId,
                    ConcurrencyStamp = userRoleId,
                    Name = "User",
                    NormalizedName = "User".ToUpper()
                }
            };

            builder.Entity<IdentityRole>().HasData(roles);
        }
    }
}
