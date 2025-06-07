using Football247.Data;
using Football247.Models.DTOs.Article;
using Football247.Models.DTOs.Image;
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

        public async Task<List<ImageDto>> CreateImageDto(List<IFormFile> BgrImgs, List<string> Captions,
            Guid ArticleDomainId, string ArticleDomainSlug)
        {
            if (BgrImgs != null && BgrImgs.Any() && Captions != null && Captions.Any())
            {
                int count = BgrImgs.Count;
                List<ImageDto> ListImageDto = new List<ImageDto>();
                ImageDto imageDto = new ImageDto();
                for (int i = 0; i < count; i++)
                {
                    var imageDomain = new Image
                    {
                        File = BgrImgs[i],
                        ImageExtension = Path.GetExtension(BgrImgs[i].FileName),
                        Caption = Captions[i],
                        DisplayOrder = i,
                        ArticleId = ArticleDomainId,
                    };
                    imageDomain = await UploadAsync(imageDomain, ArticleDomainSlug); 

                    imageDto = new ImageDto
                    {
                        ImageUrl = imageDomain.ImageUrl,
                        Caption = imageDomain.Caption,
                        DisplayOrder = imageDomain.DisplayOrder,
                    };
                    ListImageDto.Add(imageDto);
                }
                return ListImageDto; 
            }
            return null; 
        }

        public async Task<Image> UploadAsync(Image image, string ArticleSlug)
        {
            // ContentRootPath: đường dẫn gốc của ứng dụng trên máy chủ
            var localFilePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images",
                $"{ArticleSlug}-{image.DisplayOrder}{image.ImageExtension}");

            // Upload Image to Local Path
            // Mở một stream đến file đích. Ghi nội dung của file vào stream.
            using var stream = new FileStream(localFilePath, FileMode.Create);
            await image.File.CopyToAsync(stream);

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

        public void ValidateFileUpload(List<IFormFile> BgrImgs, List<string> Captions)
        {
            if (BgrImgs != null || BgrImgs.Any())
            {
                var allowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };
                for (int i = 0; i < BgrImgs.Count(); i++)
                {
                    if (BgrImgs[i].Length == 0)
                    {
                        throw new ArgumentException($"Image with caption {Captions[i]} is empty.");
                    }

                    if (BgrImgs[i].Length > 10 * 1024 * 1024) // 10MB
                    {
                        throw new ArgumentException($"Image with caption {Captions[i]} has size more than 10MB, please upload a smaller size image.");
                    }

                    var fileExtension = Path.GetExtension(BgrImgs[i].FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        throw new ArgumentException($"Image with caption {Captions[i]} has invalid image type. Only .jpg, .jpeg, .png, are allowed.");
                    }
                }
            } else
            {
                throw new ArgumentException("No files to upload. Please select at least one image file.");
            }
        }
    }
}
