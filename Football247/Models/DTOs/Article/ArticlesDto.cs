namespace Football247.Models.DTOs.Article
{
    public class ArticlesDto
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public int Priority { get; set; }

        public string BgrImg { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
