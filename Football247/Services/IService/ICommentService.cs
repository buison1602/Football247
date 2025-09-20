using Football247.Models.DTOs.Comment;

namespace Football247.Services.IService
{
    public interface ICommentService
    {
        Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId);

        Task<CommentDto> PostCommentAsync(AddCommentRequestDto addCommentRequestDto, string userId);
    }
}
