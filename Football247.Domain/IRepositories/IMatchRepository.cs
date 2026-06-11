using Football247.Repositories.IRepository;

namespace Football247.Domain.IRepositories
{
    public interface IMatchRepository : IRepository<Entities.Match>
    {
        Task<Entities.Match> UpdateAsync(Guid id, Entities.Match match);

    }
}
