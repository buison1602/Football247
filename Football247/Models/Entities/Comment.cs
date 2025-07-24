using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football247.Models.Entities
{
    public class Comment
    {
        [Key] 
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)] 
        public string Content { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 

        [Required]
        public string CreatorId { get; set; } 
        [ForeignKey("CreatorId")]
        public ApplicationUser Creator { get; set; } 

        [Required]
        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }
    }
}
