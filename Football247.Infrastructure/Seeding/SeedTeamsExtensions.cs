using Football247.Domain.Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Football247.Infrastructure.Seeding
{
    public static class SeedTeamsExtensions
    {
        public static async Task SeedTeamsAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<Football247DbContext>();

            // Đã có dữ liệu thì bỏ qua, tránh seed trùng
            if (await db.Teams.AnyAsync())
                return;

            var systemUserId = Guid.Parse("00000000-0000-0000-0000-000000000000");
            var createdDate = DateTime.UtcNow;

            var teams = new List<Team>
            {
                CreateTeam("00000000-0000-0000-0000-000000000758", "Uruguay", "https://crests.football-data.org/758.svg", "uruguay", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000759", "Germany", "https://crests.football-data.org/759.svg", "germany", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000760", "Spain", "https://crests.football-data.org/760.svg", "spain", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000761", "Paraguay", "https://crests.football-data.org/761.svg", "paraguay", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000762", "Argentina", "https://crests.football-data.org/762.png", "argentina", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000763", "Ghana", "https://crests.football-data.org/ghana.svg", "ghana", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000764", "Brazil", "https://crests.football-data.org/764.svg", "brazil", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000765", "Portugal", "https://crests.football-data.org/765.svg", "portugal", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000766", "Japan", "https://crests.football-data.org/766.svg", "japan", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000769", "Mexico", "https://crests.football-data.org/769.svg", "mexico", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000770", "England", "https://crests.football-data.org/770.svg", "england", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000771", "United States", "https://crests.football-data.org/usa.svg", "united-states", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000772", "Korea Republic", "https://crests.football-data.org/772.png", "korea-republic", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000773", "France", "https://crests.football-data.org/773.svg", "france", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000774", "South Africa", "https://crests.football-data.org/9396.svg", "south-africa", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000778", "Algeria", "https://crests.football-data.org/algeria.svg", "algeria", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000779", "Australia", "https://crests.football-data.org/779.svg", "australia", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000783", "New Zealand", "https://crests.football-data.org/783.svg", "new-zealand", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000788", "Switzerland", "https://crests.football-data.org/788.svg", "switzerland", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000791", "Ecuador", "https://crests.football-data.org/791.svg", "ecuador", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000792", "Sweden", "https://crests.football-data.org/792.svg", "sweden", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000798", "Czechia", "https://crests.football-data.org/798.svg", "czechia", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000799", "Croatia", "https://crests.football-data.org/799.svg", "croatia", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000801", "Saudi Arabia", "https://crests.football-data.org/saudi_arabia.svg", "saudi-arabia", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000802", "Tunisia", "https://crests.football-data.org/tunisia.svg", "tunisia", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000803", "Turkey", "https://crests.football-data.org/803.svg", "turkey", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000804", "Senegal", "https://crests.football-data.org/senegal.svg", "senegal", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000805", "Belgium", "https://crests.football-data.org/805.svg", "belgium", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000815", "Morocco", "https://crests.football-data.org/morocco.svg", "morocco", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000816", "Austria", "https://crests.football-data.org/816.svg", "austria", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000818", "Colombia", "https://crests.football-data.org/818.svg", "colombia", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000825", "Egypt", "https://crests.football-data.org/825.svg", "egypt", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000828", "Canada", "https://crests.football-data.org/canada.svg", "canada", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000836", "Haiti", "https://crests.football-data.org/haiti.svg", "haiti", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000000840", "Iran", "https://crests.football-data.org/iran.svg", "iran", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000001060", "Bosnia-Herzegovina", "https://crests.football-data.org/bosnia.svg", "bosnia-herzegovina", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000001836", "Panama", "https://crests.football-data.org/panama.svg", "panama", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000001930", "Cape Verde Islands", "https://crests.football-data.org/cape_verde.svg", "cape-verde-islands", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000001934", "Congo DR", "https://crests.football-data.org/congo_dr.svg", "congo-dr", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000001935", "Ivory Coast", "https://crests.football-data.org/787.svg", "ivory-coast", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000008030", "Qatar", "https://crests.football-data.org/8030.svg", "qatar", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000008049", "Jordan", "https://crests.football-data.org/8049.png", "jordan", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000008062", "Iraq", "https://crests.football-data.org/iraq.svg", "iraq", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000008070", "Uzbekistan", "https://crests.football-data.org/8070.png", "uzbekistan", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000008601", "Netherlands", "https://crests.football-data.org/8601.svg", "netherlands", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000008872", "Norway", "https://crests.football-data.org/813.svg", "norway", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000008873", "Scotland", "https://crests.football-data.org/814.svg", "scotland", systemUserId, createdDate),
                CreateTeam("00000000-0000-0000-0000-000000009460", "Curaçao", "https://crests.football-data.org/curacao.svg", "curacao", systemUserId, createdDate),
            };

            await db.Teams.AddRangeAsync(teams);
            await db.SaveChangesAsync();
        }

        private static Team CreateTeam(
            string id,
            string name,
            string logoUrl,
            string slug,
            Guid createdUserId,
            DateTime createdDate)
        {
            return new Team
            {
                Id = Guid.Parse(id),
                Name = name,
                LogoUrl = logoUrl,
                Slug = slug,
                CreatedUserId = createdUserId,
                CreatedFullName = "System",
                CreatedDate = createdDate,
                IsDeleted = false,
            };
        }
    }
}
