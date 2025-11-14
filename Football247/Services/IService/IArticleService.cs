using Football247.Models.DTOs.Article;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;

namespace Football247.Services.IService
{
    public interface IArticleService
    {
        Task<ArticleDto> CreateAsync(AddArticleRequestDto addArticleRequestDto);

        Task<ArticleDto> UpdateAsync(Guid id, UpdateArticleRequestDto updateArticleRequestDto);

        Task<ArticleDto?> DeleteAsync(Guid id);


        Task<List<ArticlesDto>> GetByCategoryAsync(string categorySlug, int page);

        Task<List<ArticlesDto>> GetByTagAsync(string tagSlug, int page);

        Task<ArticleDto> GetBySlugAsync(string articleSlug);

        Task<List<ArticlesDto>> Get5ArticlesAsync();

        Task<List<ArticlesDto>> GetAllAsync();
    }
}
