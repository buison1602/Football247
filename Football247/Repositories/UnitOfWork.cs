using Football247.Data;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Hosting; 
using Microsoft.AspNetCore.Http; 

namespace Football247.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Football247DbContext _db;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IArticleRepository ArticleRepository { get; private set; }
        public ITagRepository TagRepository { get; private set; }
        public IImageRepository ImageRepository { get; private set; }

        public UnitOfWork(Football247DbContext db, IWebHostEnvironment _webHostEnvironment,
            IHttpContextAccessor _httpContextAccessor)
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            ArticleRepository = new ArticleRepository(_db, _webHostEnvironment);
            TagRepository = new TagRepository(_db);
            ImageRepository = new ImageRepository(_db, _webHostEnvironment, _httpContextAccessor);
        }
        public Task SaveAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
