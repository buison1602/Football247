using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class ArticleTag
    {
        [Required]
        public Guid ArticleId { get; set; }
        public Article Article { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
