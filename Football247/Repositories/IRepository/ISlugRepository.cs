namespace Football247.Repositories.IRepository
{
    public interface ISlugRepository<T> where T : class
    {
        Task<T> GetBySlugAsync(string slug);
    }
}
