using Football247.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class Tag : BaseEntity
    {
        [Key]
        public Guid Id { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Slug { get; set; }

        // Navigation
        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
