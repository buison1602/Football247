namespace Football247.Models.DTOs.Comment
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatorId { get; set; }
        public string CreatorName { get; set; }
        public Guid ArticleId { get; set; }
    }
}
