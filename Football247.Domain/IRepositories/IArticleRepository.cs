using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Models.Entities;

namespace Football247.Repositories.IRepository
{
    public interface IArticleRepository : IRepository<Article>, ISlugRepository<ArticleDto>
    {
        Task<Article> UpdateAsync(Guid id, Article article);

        Task<List<ArticlesDto>> GetByCategoryAsync(string categorySlug, int page);

        Task<List<ArticlesDto>> GetByTagAsync(string tagSlug, int page);
        Task<List<ArticlesDto>> Get5ArticlesAsync();
    }
}
