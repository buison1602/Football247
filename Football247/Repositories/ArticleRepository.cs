using Football247.Data;
using Football247.Helper;
using Football247.Models.DTOs.Article;
using Football247.Models.DTOs.Tag;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Football247DbContext _db;
        private readonly ILogger<ArticleRepository> _logger;

        public ArticleRepository(Football247DbContext db, IWebHostEnvironment webHostEnvironment, ILogger<ArticleRepository> logger) : base(db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
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
                .OrderByDescending(a => a.CreatedAt)                
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(a => new ArticlesDto     
                    {
                        Id = a.Id,
                        Title = a.Title,
                        Slug = a.Slug,
                        Description = a.Description,
                        Priority = a.Priority,
                        CreatedAt = a.CreatedAt,
                        // Tự xử lý logic Map tay ở đây
                        BgrImg = a.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault(),
                        Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList()
                    })
                .ToListAsync();
        }

        public async Task<ArticleDto?> GetBySlugAsync(string articleSlug)
        {
            //var existingEntity = await _db.Articles
            //    .AsNoTracking()
            //    .Include(a => a.Images)
            //    .Include(a => a.Category)
            //    .Include(a => a.Creator)
            //    .Include(a => a.ArticleTags)
            //        // Tải đối tượng Tag liên quan đến TỪNG ArticleTag trong danh sách ArticleTags
            //        .ThenInclude(at => at.Tag) 
            //    .FirstOrDefaultAsync(u => u.Slug == articleSlug);
            //if (existingEntity == null)
            //{
            //    return new Article();
            //}
            //return existingEntity;

            return await _db.Articles
                .AsNoTracking()
                .Where(a => a.Slug == articleSlug)
                .Select(a => new ArticleDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    Content = a.Content,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedAt,
                    CreatorId = Guid.Parse(a.CreatorId),
                    CreatorName = a.Creator.UserName,
                    BgrImg = a.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).ToList(),
                    Tags = a.ArticleTags.Select(at => new TagDto
                    {
                        // Giả sử TagDto có Id và Name
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
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .Select(a => new ArticlesDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedAt,
                    BgrImg = a.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault(),
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
                .Include(a => a.Creator)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        // V
        public async Task<List<ArticlesDto>> Get5ArticlesAsync()
        {
            int limit = MaximumArticles.FiveArticles;

            return await _db.Articles
                .AsNoTracking()
                .OrderByDescending(a => a.CreatedAt)
                .Take(limit)
                .Select(a => new ArticlesDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedAt,
                    BgrImg = a.Images.OrderBy(i => i.DisplayOrder).Select(i => i.ImageUrl).FirstOrDefault(),
                    Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList()
                })
                .ToListAsync();
        }

        public async Task<Article> UpdateAsync(Guid id, Article category)
        {
            var existingEntity = await _db.Articles.FirstOrDefaultAsync(u => u.Id == id);
            if (existingEntity == null)
            {
                return new Article();
            }

            // Map DTO to Domain Model 
            existingEntity.Title = category.Title;
            existingEntity.Slug = category.Slug;
            existingEntity.Description = category.Description;
            existingEntity.Content = category.Content;
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

            // Tải Category và Creator để sử dụng sau này (nếu cần)
            await _db.Entry(entity).Reference(a => a.Category).LoadAsync();
            await _db.Entry(entity).Reference(a => a.Creator).LoadAsync();

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

            // Xóa thủ công các Images liên quan
            if (existingEntity.Images != null && existingEntity.Images.Any())
            {
                foreach (var image in existingEntity.Images)
                {
                    // Lấy tên file từ URL. Ví dụ: "https://localhost:7087/Images/articleDto-0.jpg" -> "articleDto-0.jpg"
                    var fileName = Path.GetFileName(image.ImageUrl);
                    // Xây dựng đường dẫn vật lý đến file trong thư mục /Images/
                    var imagePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images", fileName);

                    if (File.Exists(imagePath))
                    {
                        try
                        {
                            File.Delete(imagePath);
                        }
                        catch (IOException ex)
                        {
                            // Xử lý lỗi nếu file đang được sử dụng hoặc có vấn đề về quyền truy cập
                            _logger.LogError($"Error deleting image file {imagePath}: {ex.Message}");
                            // Tùy thuộc vào yêu cầu, bạn có thể ném lại ngoại lệ hoặc bỏ qua để tiếp tục xóa Article trong DB
                        }
                    }
                }
                _db.Images.RemoveRange(existingEntity.Images);
            }

            // Xóa thủ công các ArticleTags liên quan
            if (existingEntity.ArticleTags != null && existingEntity.ArticleTags.Any())
            {
                _db.ArticleTags.RemoveRange(existingEntity.ArticleTags);
            }

            // Xóa Article chính
            _db.Articles.Remove(existingEntity);
            await _db.SaveChangesAsync();

            return existingEntity;
        }
    }
}
