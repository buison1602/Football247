using Football247.Data;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly Football247DbContext _db;
        public CategoryRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<Category> GetBySlugAsync(string slug)
        {
            var existingEntity = await _db.Categories.FirstOrDefaultAsync(u => u.Slug == slug);
            if (existingEntity == null)
            {
                return null;
            }
            return existingEntity;
        }

        public async Task<Category> UpdateAsync(Guid id, Category category)
        {
            var existingEntity = await _db.Categories.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.Name = category.Name;
            existingEntity.Slug = category.Slug;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
