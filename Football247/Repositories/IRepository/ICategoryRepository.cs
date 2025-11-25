using Football247.Models.DTOs.Category;
using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface ICategoryRepository : IRepository<Category>, 
        ISlugRepository<CategoryDto>, 
        INameRepository<CategoryDto>
    {
        Task<Category> UpdateAsync(Guid id, Category category);
    }
}
