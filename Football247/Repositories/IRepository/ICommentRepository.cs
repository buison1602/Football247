using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface ICommentRepository : IRepository<Comment>
    {
        Task<List<Comment>> GetCommentsByArticleIdAsync(Guid articleId);
    }
}
