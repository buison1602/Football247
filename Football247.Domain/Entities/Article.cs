using Football247.Domain.Entities;
using Shared.Enum;
using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        
        public string Slug { get; set; }
        
        public string Description { get; set; }

        public string BgrImage { get; set; }

        public string Content { get; set; }
        
        public int Priority { get; set; } // giá trị là 1(bình thường) - 2(cao)
        
        public int ViewCount { get; set; }

        public bool IsApproved { get; set; } = false;

        public EnumStatusArticle Status { get; set; } = EnumStatusArticle.Draft;

        public Guid CategoryId { get; set; }

        public Guid? TeamId { get; set; }

        // Navigation
        public ICollection<Image> Images { get; set; }

        public Category Category { get; set; }

        public Team? Team { get; set; }

        public ICollection<ArticleTag> ArticleTags { get; set; }
    }
}
