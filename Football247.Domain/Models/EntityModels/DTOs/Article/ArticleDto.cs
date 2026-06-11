using Football247.Domain.Models.EntityModels.DTOs.Image;
using Football247.Domain.Models.EntityModels.DTOs.Tag;

namespace Football247.Domain.Models.EntityModels.DTOs.Article
{
    public class ArticleDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public string BgrImage { get; set; }

        public string Content { get; set; }

        public int Priority { get; set; }

        public int ViewCount { get; set; }

        public byte IsApproved { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public Guid CreatedUserId { get; set; }

        public string CreatedFullName { get; set; } 

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }
        
        public List<TagDto> Tags { get; set; } = new List<TagDto>();
    }
}
