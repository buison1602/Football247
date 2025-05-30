using Football247.Models.DTOs.Tag;

namespace Football247.Models.DTOs.Article
{
    public class ArticleDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public int Priority { get; set; }

        public List<string> BgrImg { get; set; }

        public int ViewCount { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public byte IsApproved { get; set; }

        public Guid CreatorId { get; set; }
        public string CreatorName { get; set; } 

        public Guid CategoryId { get; set; }
        public string CategoryName { get; set; }

        // Tags
        //public List<string> Tags { get; set; } = new();
        public List<TagDto> Tags { get; set; } = new List<TagDto>();

    }
}
