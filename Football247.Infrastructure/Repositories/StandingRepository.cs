using Football247.Domain.Entities;
using Football247.Domain.IRepositories;
using Football247.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Repositories
{
    public class StandingRepository : Repository<Standing>, IStandingRepository
    {
        private readonly Football247DbContext _db;

        public StandingRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Standing> UpdateAsync(Guid id, Standing standing)
        {
            var existingEntity = await _db.Standings.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.CompetitionCode = standing.CompetitionCode;
            existingEntity.Season = standing.Season;
            existingEntity.Position = standing.Position;
            existingEntity.TeamExternalId = standing.TeamExternalId;
            existingEntity.TeamName = standing.TeamName;
            existingEntity.TeamShortName = standing.TeamShortName;
            existingEntity.TeamCrest = standing.TeamCrest;
            existingEntity.PlayedGames = standing.PlayedGames;
            existingEntity.Won = standing.Won;
            existingEntity.Draw = standing.Draw;
            existingEntity.Lost = standing.Lost;
            existingEntity.GoalDifference = standing.GoalDifference;
            existingEntity.Points = standing.Points;


            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}

