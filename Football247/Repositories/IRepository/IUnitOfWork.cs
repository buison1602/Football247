namespace Football247.Repositories.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository CategoryRepository { get; }
        Task SaveAsync();
    }
}
