using Football247.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Football247.Infrastructure.Configs
{
    public class StandingEntityTypeConfiguration : IEntityTypeConfiguration<Standing>
    {
        public void Configure(EntityTypeBuilder<Standing> builder)
        {
            builder.HasIndex(s => new { s.CompetitionCode, s.Season, s.Position })
                .IsUnique()
                .HasDatabaseName("IX_Standings_Competition_Season_Position");
        }
    }
}
