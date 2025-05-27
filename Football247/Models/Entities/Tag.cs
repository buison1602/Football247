using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        // Navigation
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
