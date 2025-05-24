using Football247.Data;
using Football247.Helper;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        private readonly Football247DbContext _db;
        public ArticleRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<Article>> GetByCategoryAsync(string categorySlug, int page)
        {
            int limit = MaximumArticles.Limit;
            List<Article> articles = await _db.Articles
                .Include("Category")
                .Include("Creator")
                .Where(e => e.Category.Slug == categorySlug)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return articles;
        }

        public async Task<Article> GetBySlugAsync(string slug)
        {
            var existingEntity = await _db.Articles
                .Include("Category")
                .Include("Creator")
                .FirstOrDefaultAsync(u => u.Slug == slug);
            if (existingEntity == null)
            {
                return null;
            }
            return existingEntity;
        }

        public async Task<List<Article>> GetByTagAsync(string tagSlug, int page)
        {
            int limit = MaximumArticles.Limit;
            List<Article> articles = await _db.Articles
                .Include("Category")
                .Include("Creator")
                .Where(e => e.Category.Slug == tagSlug)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return articles;
        }

        public async Task<Article> UpdateAsync(Guid id, Article category)
        {
            var existingEntity = await _db.Articles
                .Include("Category")
                .Include("Creator")
                .FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return null;
            }

            // Map DTO to Domain Model 
            existingEntity.Title = category.Title;
            existingEntity.Slug = category.Slug;
            existingEntity.Description = category.Description;
            existingEntity.Content = category.Content;
            existingEntity.BgrImg = category.BgrImg;
            existingEntity.ViewCount = category.ViewCount;
            existingEntity.Slug = category.Slug;
            existingEntity.IsApproved = category.IsApproved;
            existingEntity.UpdatedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return existingEntity;
        }

        public override async Task<Article> CreateAsync(Article entity)
        {
            await _db.Articles.AddAsync(entity);
            await _db.SaveChangesAsync();

            // Tải Category để sử dụng sau này (nếu cần)
            await _db.Entry(entity).Reference(a => a.Category).LoadAsync();
            await _db.Entry(entity).Reference(a => a.Creator).LoadAsync();

            return entity;
        }

        public override async Task<List<Article>> GetAllAsync()
        {
            return await _db.Articles
                .Include("Category")
                .Include("Creator")
                .ToListAsync();
        }
    }
}
