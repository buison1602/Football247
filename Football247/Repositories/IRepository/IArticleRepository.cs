using Football247.Helper;
using Football247.Models.DTOs.Article;
using Football247.Models.Entities;
using Microsoft.AspNetCore.Mvc;

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
