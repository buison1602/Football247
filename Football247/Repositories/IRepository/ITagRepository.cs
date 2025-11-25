using Football247.Models.DTOs.Tag;
using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface ITagRepository : IRepository<Tag>, 
        ISlugRepository<TagDto>,
        INameRepository<TagDto>
    {
        Task<Tag> UpdateAsync(Guid id, Tag tag);
    }
}
