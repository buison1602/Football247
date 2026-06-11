namespace Football247.Domain.Models.CommandModels.CommentCmdModel
{
    public class CreateCommentCommandModel
    {
        public string Content { get; set; }
        public Guid ArticleId { get; set; }
    }
}
