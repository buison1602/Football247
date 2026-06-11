namespace Football247.Domain.Models.EntityModels.DTOs.Comment
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid CreatorId { get; set; }
        public string CreatorName { get; set; }
        public Guid ArticleId { get; set; }
    }
}
