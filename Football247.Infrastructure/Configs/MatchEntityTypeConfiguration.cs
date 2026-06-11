using Football247.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Configs
{
    public class MatchEntityTypeConfiguration : IEntityTypeConfiguration<Match>
    {
        public void Configure(EntityTypeBuilder<Match> builder)
        {
            builder.Property(e => e.Status)
                .HasMaxLength(20)
                .HasConversion(
                    v => v.ToString().ToUpper(),
                    v => Enum.Parse<EnumMatchStatus>(v, true)
                );

            builder.HasIndex(m => m.ExternalId)
                .IsUnique()
                .HasDatabaseName("IX_Matches_ExternalId");

            builder.HasIndex(m => new { m.CompetitionCode, m.Season, m.Status })
                .HasDatabaseName("IX_Matches_Listing_Optimized");

            builder.HasIndex(m => m.UtcDate);
        }
    }
}
