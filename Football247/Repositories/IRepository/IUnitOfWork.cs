namespace Football247.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        IArticleRepository ArticleRepository { get; }
        ITagRepository TagRepository { get; }
        IImageRepository ImageRepository { get; }
        ITokenRepository TokenRepository { get; }
        Task SaveAsync();
    }
}
