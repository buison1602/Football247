namespace Football247.Repositories.IRepository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> Queryable { get; }
        IQueryable<T> ReadQueryable { get; }
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<T?> DeleteAsync(Guid id);
    }
}
