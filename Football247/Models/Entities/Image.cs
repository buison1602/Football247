using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class Image
    {
        [Key]
        public Guid Id { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        public string ImageExtension { get; set; }

        public string ImageUrl { get; set; }

        public string? Caption { get; set; } 

        public int DisplayOrder { get; set; } = 0;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public Guid ArticleId { get; set; }

        // 2. Navigation 
        [ForeignKey("ArticleId")]
        public Article Article { get; set; }
    }
}
