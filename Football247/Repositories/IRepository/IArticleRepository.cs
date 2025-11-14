using Football247.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Football247.Helper;

namespace Football247.Repositories.IRepository
{
    public interface IArticleRepository : IRepository<Article>, ISlugRepository<Article>
    {
        Task<Article> UpdateAsync(Guid id, Article article);

        Task<List<Article>> GetByCategoryAsync(string categorySlug, int page);

        Task<List<Article>> GetByTagAsync(string tagSlug, int page);
        Task<List<Article>> Get5ArticlesAsync();
    }
}
