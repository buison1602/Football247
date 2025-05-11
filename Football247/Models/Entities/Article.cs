using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class Article
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Title { get; set; }
        
        [Required]
        public string Slug { get; set; }
        
        [Required]
        public string Description { get; set; }
        
        [Required]
        public string Content { get; set; }
        
        [Required]
        public int Priority { get; set; }
        
        [Required]
        public string BgrImg { get; set; }

        public int ViewCount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public byte IsApproved { get; set; }

        public Guid? CreatorId { get; set; }
        public Guid CategoryId { get; set; }

        // Navigation
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
