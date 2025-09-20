using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface ICategoryRepository : IRepository<Category>, 
        ISlugRepository<Category>, 
        INameRepository<Category>
    {
        Task<Category> UpdateAsync(Guid id, Category category);
    }
}
