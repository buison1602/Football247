using Football247.Data;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class TagRepository : Repository<Tag>, ITagRepository
    {
        private readonly Football247DbContext _db;
        public TagRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Tag> GetBySlugAsync(string slug)
        {
            var existingEntity = await _db.Tags.FirstOrDefaultAsync(u => u.Slug == slug);
            if (existingEntity == null)
            {
                return null;
            }
            return existingEntity;
        }

        public async Task<Tag> UpdateAsync(Guid id, Tag tag)
        {
            var existingEntity = await _db.Tags.FirstOrDefaultAsync(t => t.Id == id);
            if (existingEntity == null) 
            {
                return null;
            }

            existingEntity.Slug = tag.Slug;
            existingEntity.Name = tag.Name;
            existingEntity.UpdatedAt = DateTime.Now;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
