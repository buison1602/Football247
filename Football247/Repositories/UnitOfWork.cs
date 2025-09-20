using Football247.Data;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Football247.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Football247DbContext _db;


        // Khai báo các backing field kiểu Lazy<T>
        private readonly Lazy<ICategoryRepository> _categoryRepository;
        private readonly Lazy<IArticleRepository> _articleRepository;
        private readonly Lazy<ITagRepository> _tagRepository;
        private readonly Lazy<IImageRepository> _imageRepository;
        private readonly Lazy<ITokenRepository> _tokenRepository;
        private readonly Lazy<ICommentRepository> _commentRepository;

        public ICategoryRepository CategoryRepository => _categoryRepository.Value;
        public IArticleRepository ArticleRepository => _articleRepository.Value;
        public ITagRepository TagRepository => _tagRepository.Value;
        public IImageRepository ImageRepository => _imageRepository.Value;
        public ITokenRepository TokenRepository => _tokenRepository.Value;
        public ICommentRepository CommentRepository => _commentRepository.Value;


        public UnitOfWork(
            Football247DbContext db, 
            IWebHostEnvironment _webHostEnvironment,
            IHttpContextAccessor _httpContextAccessor, 
            IConfiguration configuration,
            ILoggerFactory loggerFactory,
            UserManager<ApplicationUser> userManager)
        {
            _db = db;

            _categoryRepository = new Lazy<ICategoryRepository>(() => new CategoryRepository(_db));
            _articleRepository = new Lazy<IArticleRepository>(() => new ArticleRepository(_db, _webHostEnvironment, loggerFactory.CreateLogger<ArticleRepository>()));
            _tagRepository = new Lazy<ITagRepository>(() => new TagRepository(_db));
            _imageRepository = new Lazy<IImageRepository>(() => new ImageRepository(_db, _webHostEnvironment, _httpContextAccessor));
            _tokenRepository = new Lazy<ITokenRepository>(() => new TokenRepository(configuration, _db, userManager));
            _commentRepository = new Lazy<ICommentRepository>(() => new CommentRepository(_db));
        }

        public Task SaveAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
