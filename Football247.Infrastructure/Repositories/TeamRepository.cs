using Football247.Domain.Entities;
using Football247.Domain.IRepositories;
using Football247.Domain.Models.EntityModels.DTOs.Team;
using Football247.Models.Entities;
using Football247.Repositories;
using Football247.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Repositories
{
    public class TeamRepository : Repository<Team>, ITeamRepository
    {
        private readonly Football247DbContext _db;

        public TeamRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<TeamDto?> GetBySlugAsync(string slug)
        {
            return await _db.Teams
                .Where(t => t.Slug == slug)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Slug = t.Slug
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Team?> UpdateAsync(Guid id, Team team)
        {
            var existingEntity = await _db.Teams.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.Name = team.Name;
            existingEntity.Slug = team.Slug;
            existingEntity.UpdatedDate = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
