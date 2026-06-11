using Football247.Domain.IRepositories;
using Football247.Domain.IRepositories.Store;
using Football247.Infrastructure;
using Football247.Infrastructure.Repositories;
using Football247.Infrastructure.Repositories.Stores;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Football247.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Football247DbContext _db;
        private IDbContextTransaction _currentTransaction;


        // Khai báo các backing field kiểu Lazy<T>
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IArticleRepository> _articleRepository;
        private readonly Lazy<ITagRepository> _tagRepository;
        private readonly Lazy<IImageRepository> _imageRepository;
        private readonly Lazy<ITokenRepository> _tokenRepository;
        private readonly Lazy<ICommentRepository> _commentRepository;
        private readonly Lazy<IRoleRepository> _roleRepository;
        private readonly Lazy<INotificationMessageRepository> _notificationMessageRepository;
        private readonly Lazy<ITeamRepository> _teamRepository;
        private readonly Lazy<IMatchRepository> _matchRepository;
        private readonly Lazy<IStandingRepository> _standingRepository;

        private readonly Lazy<ICartRepository> _cartRepository;
        private readonly Lazy<ICartItemRepository> _cartItemRepository;
        private readonly Lazy<IOrderRepository> _orderRepository;
        private readonly Lazy<IOrderItemRepository> _orderItemRepository;
        private readonly Lazy<IProductRepository> _productRepository;
        private readonly Lazy<IProductCategoryRepository> _productCategoryRepository;



        public ICategoryRepository CategoryRepository => _categoryRepository.Value;
        public IArticleRepository ArticleRepository => _articleRepository.Value;
        public ITagRepository TagRepository => _tagRepository.Value;
        public IImageRepository ImageRepository => _imageRepository.Value;
        public ITokenRepository TokenRepository => _tokenRepository.Value;
        public ICommentRepository CommentRepository => _commentRepository.Value;
        public IRoleRepository RoleRepository => _roleRepository.Value;
        public INotificationMessageRepository NotificationMessageRepository => _notificationMessageRepository.Value;
        public ITeamRepository TeamRepository => _teamRepository.Value;
        public IMatchRepository MatchRepository => _matchRepository.Value;
        public IStandingRepository StandingRepository => _standingRepository.Value;

        public ICartRepository CartRepository => _cartRepository.Value;
        public ICartItemRepository CartItemRepository => _cartItemRepository.Value;
        public IOrderRepository OrderRepository => _orderRepository.Value;
        public IOrderItemRepository OrderItemRepository => _orderItemRepository.Value;
        public IProductRepository ProductRepository => _productRepository.Value;
        public IProductCategoryRepository ProductCategoryRepository => _productCategoryRepository.Value;


        public UnitOfWork(
            Football247DbContext db, 
            IWebHostEnvironment _webHostEnvironment,
            IHttpContextAccessor _httpContextAccessor, 
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<Guid>> roleManager)
        {
            _db = db;

            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(_db));
            _articleRepository = new Lazy<IArticleRepository>(() => new ArticleRepository(_db));
            _tagRepository = new Lazy<ITagRepository>(() => new TagRepository(_db));
            _imageRepository = new Lazy<IImageRepository>(() => new ImageRepository(_db, _webHostEnvironment, _httpContextAccessor));
            _tokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(configuration, _db, userManager));
            _commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(_db));
            _roleRepository = new Lazy<IRoleRepository>(() => new RoleRepository(roleManager));
            _notificationMessageRepository = new Lazy<INotificationMessageRepository>(() => new NotificationMessageRepository(_db));
            _teamRepository = new Lazy<ITeamRepository>(() => new TeamRepository(_db));
            _matchRepository = new Lazy<IMatchRepository>(() => new MatchRepository(_db));
            _standingRepository = new Lazy<IStandingRepository>(() => new StandingRepository(_db));

            _cartRepository = new Lazy<ICartRepository>(() => new CartRepository(_db));
            _cartItemRepository = new Lazy<ICartItemRepository>(() => new CartItemRepository(_db));
            _orderRepository = new Lazy<IOrderRepository>(() => new OrderRepository(_db));
            _orderItemRepository = new Lazy<IOrderItemRepository>(() => new OrderItemRepository(_db));
            _productRepository = new Lazy<IProductRepository>(() => new ProductRepository(_db));
            _productCategoryRepository = new Lazy<IProductCategoryRepository>(() => new ProductCategoryRepository(_db));
        }

        public Task SaveAsync()
        {
            return _db.SaveChangesAsync();
        }


        // Quản lý Transaction thủ công
        public async Task BeginTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                return;
            }
            _currentTransaction = await _db.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _currentTransaction.CommitAsync();
            }
            catch
            {
                await RollbackTransactionAsync();
                throw;
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }

        public async Task RollbackTransactionAsync()
        {
            try
            {
                await _currentTransaction.RollbackAsync();
            }
            finally
            {
                if (_currentTransaction != null)
                {
                    await _currentTransaction.DisposeAsync();
                    _currentTransaction = null;
                }
            }
        }


        public void Dispose()
        {
            _currentTransaction?.Dispose();

            _db.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}
