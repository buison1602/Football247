using System.ComponentModel.DataAnnotations;

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
        
        public List<string> BgrImg { get; set; }

        public int ViewCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public byte IsApproved { get; set; }

        public string? CreatorId { get; set; }
        public Guid CategoryId { get; set; }

        // Navigation
        public User Creator { get; set; } 

        public Category Category { get; set; }

        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
