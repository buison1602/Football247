using Football247.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Football247.Models.Entities
{
    public class Comment : BaseEntity
    {
        [Key] 
        public Guid Id { get; set; }

        [Required]
        [MaxLength(500)] 
        public string Content { get; set; }

        [Required]
        public Guid ArticleId { get; set; }
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }

        public bool? IsReported { get; set; } = false;
    }
}
