using Microsoft.EntityFrameworkCore;

namespace Football247.Data
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
