using Football247.Models.DTOs.Comment;
using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId);
    }
}
