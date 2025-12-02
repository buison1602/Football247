namespace Football247.Repositories.IRepository
{
    public interface IUnitOfWork : IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IArticleRepository ArticleRepository { get; }
        ITagRepository TagRepository { get; }
        IImageRepository ImageRepository { get; }
        ITokenRepository TokenRepository { get; }
        ICommentRepository CommentRepository { get; }
        IRoleRepository RoleRepository { get; }

        // Quản lý Transaction thủ công
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task SaveAsync();
    }
}
