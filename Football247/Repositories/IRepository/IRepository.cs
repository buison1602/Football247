namespace Football247.Repositories.IRepository
{
    public interface IRepository<T> where T : class
    {
        // Put the Update method in the IRepository of each object because
        // each entity has its own update logic.
        Task<List<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> CreateAsync(T entity);
        Task<T?> DeleteAsync(Guid id);
    }
}
