using System.ComponentModel.DataAnnotations;
using static System.Net.Mime.MediaTypeNames;

namespace Football247.Models.Entities
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }

        public string Title { get; set; }
        
        public string Slug { get; set; }
        
        public string Description { get; set; }
        
        public string Content { get; set; }
        
        public int Priority { get; set; }
        
        public int ViewCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsApproved { get; set; }

        public string? CreatorId { get; set; }
        public Guid CategoryId { get; set; }

        // Navigation
        public ICollection<Image> Images { get; set; }

        public User Creator { get; set; } 

        public Category Category { get; set; }

        // Dùng ArticleTags để tải CÁC Tag liên quan đến Article
        // Nhập vào TagId và ArticleId nên sẽ tạo ra CÁC bản ghi ArticleTag - Xem ở ArticleProfile
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
