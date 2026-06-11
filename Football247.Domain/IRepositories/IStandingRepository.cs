using Football247.Domain.Entities;
using Football247.Repositories.IRepository;


namespace Football247.Domain.IRepositories
{
    public interface IStandingRepository : IRepository<Standing>
    {
        Task<Entities.Standing> UpdateAsync(Guid id, Entities.Standing match);

    }
}
