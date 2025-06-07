using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface IImageRepository : IRepository<Image>
    {
        Task<Image> Upload(Image image, string ArticleSlug);
    }
}
