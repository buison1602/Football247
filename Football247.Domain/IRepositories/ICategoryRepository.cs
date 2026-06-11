using Football247.Domain.Models.EntityModels.DTOs.Category;
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
