using Football247.Domain.IRepositories;
using Football247.Domain.IRepositories.Store;

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
        INotificationMessageRepository NotificationMessageRepository { get; }
        ITeamRepository TeamRepository { get; }
        IMatchRepository MatchRepository { get; }
        IStandingRepository StandingRepository { get; }


        // Store
        ICartRepository CartRepository { get; }
        ICartItemRepository CartItemRepository { get; }
        IOrderRepository OrderRepository { get; }
        IOrderItemRepository OrderItemRepository { get; }
        IProductRepository ProductRepository { get; }
        IProductCategoryRepository ProductCategoryRepository { get; }


        // Quản lý Transaction thủ công
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();

        Task SaveAsync();
    }
}
