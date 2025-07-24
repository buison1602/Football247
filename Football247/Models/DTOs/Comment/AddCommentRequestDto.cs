namespace Football247.Models.DTOs.Comment
{
    public class AddCommentRequestDto
    {
        public string Content { get; set; }
        public Guid ArticleId { get; set; }
    }
}
