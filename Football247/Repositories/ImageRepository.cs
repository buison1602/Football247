using Football247.Data;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class ImageRepository : Repository<Image>, IImageRepository
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Football247DbContext _db;
        public ImageRepository(Football247DbContext db, 
            IWebHostEnvironment webHostEnvironment,
            IHttpContextAccessor httpContextAccessor) : base(db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Image> Upload(Image image, string ArticleSlug)
        {
            // ContentRootPath: đường dẫn gốc của ứng dụng trên máy chủ
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images",
                $"{ArticleSlug}-{image.DisplayOrder}{image.ImageExtension}");

            // Upload Image to Local Path
            // Mở một stream đến file đích. Ghi nội dung của file vào stream.
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

            // Tạo đường dẫn truy cập công khai (URL) https://localhost:1234/images/HAHA.jpg
            // Thêm builder.Services.AddHttpContextAccessor(); vào program.cs 
            var urlFilePath = $"{_httpContextAccessor.HttpContext.Request.Scheme}://" +
                $"{_httpContextAccessor.HttpContext.Request.Host}" +
                $"{_httpContextAccessor.HttpContext.Request.PathBase}" +
                $"/Images/{ArticleSlug}" +
                $"-{image.DisplayOrder}" +
                $"{image.ImageExtension}";

            image.ImageUrl = urlFilePath;

            // Add Image to the Images table 
            await _db.Images.AddAsync(image);
            await _db.SaveChangesAsync();

            return image;
        }
    }
}
