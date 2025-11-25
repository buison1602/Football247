using Football247.Data;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
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

        public async Task<CategoryDto?> GetByNameAsync(string name)
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.Name == name)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryDto?> GetBySlugAsync(string slug)
        {
            return await _db.Categories
                .AsNoTracking()
                .Where(c => c.Slug == slug)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();
        }

        public async Task<Category?> UpdateAsync(Guid id, Category category)
        {
            var existingEntity = await _db.Categories.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.Name = category.Name;
            existingEntity.Slug = category.Slug;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
