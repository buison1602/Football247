namespace Football247.Domain.Models.EntityModels.DTOs.Article
{
    public class ArticlesDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public string BgrImage { get; set; }

        public int Priority { get; set; }

        public string? TeamName { get; set; }

        public DateTime CreatedAt { get; set; }

        public List<string> Tags { get; set; } = new List<string>();
    }
}
