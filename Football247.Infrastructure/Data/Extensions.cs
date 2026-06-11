using Football247.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Football247.Application.Common.Data
{
    public static class Extensions
    {
        public static async Task ApplyMigrations(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<Football247DbContext>();
            await dbContext.Database.MigrateAsync();
        }
    }
}
