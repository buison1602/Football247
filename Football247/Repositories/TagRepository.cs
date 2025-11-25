using Football247.Data;
using Football247.Models.DTOs.Tag;
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

        public async Task<TagDto?> GetByNameAsync(string name)
        {
            return await _db.Tags
                .AsNoTracking()
                .Where(u => u.Name == name)
                .Select(u => new TagDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Slug = u.Slug,
                })
                .FirstOrDefaultAsync();
        }

        public async Task<TagDto?> GetBySlugAsync(string slug)
        {
            return await _db.Tags
                .AsNoTracking()
                .Where(u => u.Slug == slug)
                .Select(u => new TagDto
                {
                    Id = u.Id,
                    Name = u.Name,
                    Slug = u.Slug,
                })
                .FirstOrDefaultAsync();
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
