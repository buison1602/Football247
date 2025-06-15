using Football247.Data;
using Football247.Helper;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class ArticleRepository : Repository<Article>, IArticleRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly Football247DbContext _db;
        public ArticleRepository(Football247DbContext db, IWebHostEnvironment webHostEnvironment) : base(db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<List<Article>> GetByCategoryAsync(string categorySlug, int page)
        {
            int limit = MaximumArticles.Limit;
            List<Article> articles = await _db.Articles
                .Include(a => a.Images)
                .Include(a => a.Category)
                .Include(a => a.Creator)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                
                .Where(e => e.Category.Slug == categorySlug)
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return articles;
        }

        public async Task<Article> GetBySlugAsync(string slug)
        {
            var existingEntity = await _db.Articles
                .Include(a => a.Images)
                .Include(a => a.Category)
                .Include(a => a.Creator)
                .Include(a => a.ArticleTags)
                    // Tải đối tượng Tag liên quan đến TỪNG ArticleTag trong danh sách ArticleTags
                    .ThenInclude(at => at.Tag) 
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
                .Include(a => a.Images)
                .Include(a => a.Category) 
                .Include(a => a.Creator)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .Where(a => a.ArticleTags.Any(at => at.Tag.Slug == tagSlug))
                .OrderByDescending(a => a.CreatedAt)
                .Skip((page - 1) * limit)
                .Take(limit)
                .ToListAsync();

            return articles;
        }

        public async Task<Article> UpdateAsync(Guid id, Article category)
        {
            var existingEntity = await _db.Articles
                .Include(a => a.Images)
                .Include(a => a.Category)
                .Include(a => a.Creator)
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

        public override async Task<List<Article>> GetAllAsync()
        {
            return await _db.Articles
                .Include(a => a.Images)
                .Include(a => a.Category)
                .Include(a => a.Creator)
                .Include(a => a.ArticleTags)
                    .ThenInclude(at => at.Tag)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public override async Task<Article> DeleteAsync(Guid id)
        {
            var existingEntity = await _db.Articles
                 .Include(a => a.Images)
                 .Include(a => a.Category)
                 .Include(a => a.Creator)
                 .FirstOrDefaultAsync(u => u.Id == id);

            if (existingEntity == null)
            {
                return null;
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
                            // Logger.LogError($"Error deleting image file {imagePath}: {ex.Message}");
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
