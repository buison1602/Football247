using Football247.Data;
using Football247.Repositories.IRepository;

namespace Football247.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Football247DbContext _db;
        public ICategoryRepository CategoryRepository { get; private set; }
        public IArticleRepository ArticleRepository { get; private set; }
        public ITagRepository TagRepository { get; private set; }

        public UnitOfWork(Football247DbContext db)
        {
            _db = db;
            CategoryRepository = new CategoryRepository(_db);
            ArticleRepository = new ArticleRepository(_db);
            TagRepository = new TagRepository(_db);
        }
        public Task SaveAsync()
        {
            return _db.SaveChangesAsync();
        }
    }
}
