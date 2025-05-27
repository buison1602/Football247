using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface ITagRepository : IRepository<Tag>, ISlugRepository<Tag>
    {
        Task<Tag> UpdateAsync(Guid id, Tag tag);
    }
}
