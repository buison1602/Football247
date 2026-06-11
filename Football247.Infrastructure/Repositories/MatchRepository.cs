using Football247.Domain.Entities;
using Football247.Domain.IRepositories;
using Football247.Repositories;
using Microsoft.EntityFrameworkCore;


namespace Football247.Infrastructure.Repositories
{
    public class MatchRepository : Repository<Match>, IMatchRepository
    {
        private readonly Football247DbContext _db;

        public MatchRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Match> UpdateAsync(Guid id, Match match)
        {
            var existingEntity = await _db.Matches.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.ExternalId = match.ExternalId;
            existingEntity.UtcDate = match.UtcDate;
            existingEntity.Status = match.Status;
            existingEntity.Matchday = match.Matchday;
            existingEntity.HomeTeamExternalId = match.HomeTeamExternalId;
            existingEntity.HomeTeamName = match.HomeTeamName;
            existingEntity.HomeTeamShortName = match.HomeTeamShortName;
            existingEntity.HomeTeamCrest = match.HomeTeamCrest;
            existingEntity.HomeScore = match.HomeScore;
            existingEntity.AwayTeamExternalId = match.AwayTeamExternalId;
            existingEntity.AwayTeamName = match.AwayTeamName;
            existingEntity.AwayTeamShortName = match.AwayTeamShortName;
            existingEntity.AwayTeamCrest = match.AwayTeamCrest;
            existingEntity.AwayScore = match.AwayScore;
            existingEntity.CompetitionCode = match.CompetitionCode;
            existingEntity.CompetitionName = match.CompetitionName;
            existingEntity.Season = match.Season;


            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
