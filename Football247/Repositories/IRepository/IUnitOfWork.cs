namespace Football247.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IArticleRepository ArticleRepository { get; }
        ITagRepository TagRepository { get; }
        Task SaveAsync();
    }
}
