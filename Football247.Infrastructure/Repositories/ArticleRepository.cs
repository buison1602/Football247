using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Domain.Models.EntityModels.DTOs.Tag;
using Football247.Helper;
using Football247.Infrastructure;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Football247.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        private readonly Football247DbContext _db;

        public ArticleRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        // V
        public async Task<List<ArticlesDto>> GetByCategoryAsync(string categorySlug, int page)
        {
            int limit = MaximumArticles.Limit;
            var categoryId = await _db.Categories
                                      .AsNoTracking()
                                      .Where(c => c.Slug == categorySlug)
                                      .Select(c => c.Id)
                                      .FirstOrDefaultAsync();

            if (categoryId == Guid.Empty) return new List<ArticlesDto>();

            return await _db.Articles
                .AsNoTracking()
                .Where(a => a.CategoryId == categoryId && a.IsApproved) 
                .OrderByDescending(a => a.CreatedDate)                
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(a => new ArticlesDto     
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Slug = a.Slug,
                        Description = a.Description,
                        BgrImage = a.BgrImage,
                        Priority = a.Priority,
                        CreatedAt = a.CreatedDate,
                        Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList()
                    })
                .ToListAsync();
        }

        public async Task<ArticleDto?> GetBySlugAsync(string articleSlug)
        {
            return await _db.Articles
                .AsNoTracking()
                .Where(a => a.Slug == articleSlug)
                .Select(a => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    BgrImage = a.BgrImage,
                    Content = a.Content,
                    Priority = a.Priority,
                    CreatedDate = a.CreatedDate,
                    CreatedUserId = a.CreatedUserId,
                    CreatedFullName = a.CreatedFullName,
                    Tags = a.ArticleTags.Select(at => new TagDto
                    {
                        Id = at.Tag.Id,
                        Name = at.Tag.Name,
                        Slug = at.Tag.Slug
                    }).ToList(),
                })
                .FirstOrDefaultAsync();
        }

        // V
        public async Task<List<ArticlesDto>> GetByTagAsync(string tagSlug, int page)
        {
            int limit = MaximumArticles.Limit;

            var tagId = await _db.Tags
                                      .AsNoTracking()
                                      .Where(c => c.Slug == tagSlug)
                                      .Select(c => c.Id)
                                      .FirstOrDefaultAsync();

            if (tagId == Guid.Empty) return new List<ArticlesDto>();


            return await _db.Articles
                .AsNoTracking()
                .Where(a => a.ArticleTags.Any(at => at.TagId == tagId) && a.IsApproved)
                .OrderByDescending(a => a.CreatedDate)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(a => new ArticlesDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    BgrImage = a.BgrImage,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedDate,
                    Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList()
                })
                .ToListAsync();
        }

        // Chưa có page
        public override async Task<List<Article>> GetAllAsync()
        {
            int limit = MaximumArticles.Limit;

            return await _db.Articles
                .Include(a => a.Images)
                .Include(a => a.Category)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .OrderByDescending(a => a.CreatedDate)
                .Take(limit)
                .ToListAsync();
        }

        // V
        public async Task<List<ArticlesDto>> Get5ArticlesAsync()
        {
            int limit = MaximumArticles.FiveArticles;

            return await _db.Articles
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedDate)
                .Take(limit)
                .Select(a => new ArticlesDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    BgrImage = a.BgrImage,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedDate,
                    Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<Article> UpdateAsync(Guid id, Article article)
        {
            return _db.Update(article).Entity;
        }

        public override async Task<Article> CreateAsync(Article entity)
        {
            await _db.Articles.AddAsync(entity);
            await _db.SaveChangesAsync();

            // Tải Category và Creator để sử dụng sau này (nếu cần)
            await _db.Entry(entity).Reference(a => a.Category).LoadAsync();

            return entity;
        }

        public override async Task<Article> DeleteAsync(Guid id)
        {
            var existingEntity = await _db.Articles
                 .Include(a => a.Images)
                 .FirstOrDefaultAsync(u => u.Id == id);

            if (existingEntity == null)
            {
                return new Article();
            }

            if (existingEntity.ArticleTags != null && existingEntity.ArticleTags.Any())
            {
                _db.ArticleTags.RemoveRange(existingEntity.ArticleTags);
            }

            _db.Articles.Remove(existingEntity);
            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
