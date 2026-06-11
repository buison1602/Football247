using Football247.Domain.Models.EntityModels.DTOs.Comment;
using Football247.Infrastructure;
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

        public Task<List<CommentDto>> GetCommentsByArticleIdAsync(Guid articleId)
        {
            return _db.Comments
                .AsNoTracking()
                .Where(c => c.ArticleId == articleId)
                .OrderByDescending(c => c.CreatedDate)
                .Select(c => new CommentDto 
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedDate,
                    ArticleId = c.ArticleId,
                    CreatorId = c.CreatedUserId,
                    CreatorName = c.CreatedFullName
                })
                .ToListAsync();
        }
    }
}
