using Football247.Data;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Football247.Repositories
{
    public class CommentRepository : Repository<Comment>, ICommentRepository
    {
        private readonly Football247DbContext _db;
        public CommentRepository(Football247DbContext db) : base(db)
        {
            _db = db;
        }

        public Task<List<Comment>> GetCommentsByArticleIdAsync(Guid articleId)
        {
            return _db.Comments
                .Where(c => c.ArticleId == articleId)
                .Include(c => c.Creator) 
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
