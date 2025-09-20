namespace Football247.Repositories.IRepository
{
    public interface INameRepository<T> where T : class
    {
        Task<T> GetByNameAsync(string name);
    }
}
