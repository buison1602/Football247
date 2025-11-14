using AutoMapper;
using Football247.Models.DTOs.Article;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.IService;
using Microsoft.Extensions.Caching.Memory;

namespace Football247.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "articles";

        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }

        public async Task<ArticleDto> CreateAsync(AddArticleRequestDto addArticleRequestDto)
        {
            var articleExist = await _unitOfWork.ArticleRepository.GetBySlugAsync(addArticleRequestDto.Slug);
            if (articleExist != null)
            {
                throw new InvalidOperationException("Article with the same slug already exists.");
            }

            Article? articleDomain = _mapper.Map<Article>(addArticleRequestDto);

            // Validate the image file
            _unitOfWork.ImageRepository.ValidateFileUpload(addArticleRequestDto.BgrImgs, addArticleRequestDto.Captions);

            articleDomain = await _unitOfWork.ArticleRepository.CreateAsync(articleDomain);
            if (articleDomain == null)
            {
                throw new InvalidOperationException("Failed to create the Article in the database.");
            }
            _memoryCache.Remove(CacheKey);

            articleDomain = await _unitOfWork.ArticleRepository.GetBySlugAsync(articleDomain.Slug);
            ArticleDto articleDto = _mapper.Map<ArticleDto>(articleDomain);

            articleDto.Images = await _unitOfWork.ImageRepository.CreateImageDto(
                addArticleRequestDto.BgrImgs,
                addArticleRequestDto.Captions,
                articleDomain.Id,
                articleDomain.Slug);

            return articleDto;
        }


        public async Task<ArticleDto> UpdateAsync(Guid id, UpdateArticleRequestDto updateArticleRequestDto)
        {
            Article? articleDomain;

            articleDomain = await _unitOfWork.ArticleRepository.GetByIdAsync(id);
            if (articleDomain == null)
            {
                throw new InvalidOperationException("Article don't exist");
            }

            articleDomain = _mapper.Map<Article>(updateArticleRequestDto);
            articleDomain = await _unitOfWork.ArticleRepository.UpdateAsync(id, articleDomain);
            if (articleDomain == null)
            {
                throw new InvalidOperationException("Failed to update the Article in the database.");
            }
            _memoryCache.Remove(CacheKey);
            return _mapper.Map<ArticleDto>(articleDomain);
        }


        public async Task<ArticleDto?> DeleteAsync(Guid id)
        {
            Article? articleDomain;

            articleDomain = await _unitOfWork.ArticleRepository.GetByIdAsync(id);
            if (articleDomain == null)
            {
                throw new InvalidOperationException("Article don't exist");
            }

            articleDomain = await _unitOfWork.ArticleRepository.DeleteAsync(id);
            if (articleDomain == null)
            {
                throw new InvalidOperationException("Failed to delete the Article.");
            }

            await _unitOfWork.SaveAsync();

            _memoryCache.Remove(CacheKey);
            return _mapper.Map<ArticleDto>(articleDomain);
        }


        public async Task<List<ArticlesDto>> GetByCategoryAsync(string categorySlug, int page)
        {
            List<Article> articles;
            string NewCacheKey = $"articles_{categorySlug}_{page}";
            if (_memoryCache.TryGetValue(NewCacheKey, out List<Article>? data))
            {
                articles = data;
            }
            else
            {
                articles = await _unitOfWork.ArticleRepository.GetByCategoryAsync(categorySlug, page);
                if (articles == null || !articles.Any())
                {
                    return null;
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                _memoryCache.Set(NewCacheKey, articles, cacheEntryOptions);
            }

            return _mapper.Map<List<ArticlesDto>>(articles);
        }


        public async Task<List<ArticlesDto>> GetByTagAsync(string tagSlug, int page)
        {
            List<Article> articles;
            string NewCacheKey = $"articles_{tagSlug}_{page}";
            if (_memoryCache.TryGetValue(NewCacheKey, out List<Article>? data))
            {
                articles = data;
            }
            else
            {
                articles = await _unitOfWork.ArticleRepository.GetByTagAsync(tagSlug, page);
                if (articles == null || !articles.Any())
                {
                    return null;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                _memoryCache.Set(NewCacheKey, articles, cacheEntryOptions);
            }

            return _mapper.Map<List<ArticlesDto>>(articles);
        }


        public async Task<ArticleDto> GetBySlugAsync(string articleSlug)
        {
            Article? articleDomain;
            string NewCacheKey = $"articles_{articleSlug}";
            if (_memoryCache.TryGetValue(NewCacheKey, out Article? data))
            {
                articleDomain = data;
            }
            else
            {
                articleDomain = await _unitOfWork.ArticleRepository.GetBySlugAsync(articleSlug);
                if (articleDomain == null)
                {
                    return null;
                }
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                _memoryCache.Set(NewCacheKey, articleDomain, cacheEntryOptions);
            }

            return _mapper.Map<ArticleDto>(articleDomain);
        }


        public async Task<List<ArticlesDto>> Get5ArticlesAsync()
        {
            List<Article> articles = new List<Article>();
            string NewCacheKey = $"5NewArticles";
            if (_memoryCache.TryGetValue(NewCacheKey, out List<Article>? data))
            {
                articles = data;
            }
            else
            {
                articles = await _unitOfWork.ArticleRepository.Get5ArticlesAsync();
                if (articles == null || !articles.Any())
                {
                    return null;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                _memoryCache.Set(NewCacheKey, articles, cacheEntryOptions);
            }

            return _mapper.Map<List<ArticlesDto>>(articles);
        }


        public async Task<List<ArticlesDto>> GetAllAsync()
        {
            List<Article> articles = new List<Article>();

            if (_memoryCache.TryGetValue(CacheKey, out List<Article>? data))
            {
                articles = data;
            }
            else
            {
                articles = await _unitOfWork.ArticleRepository.GetAllAsync();
                if (articles == null || !articles.Any())
                {
                    return null;
                }

                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                _memoryCache.Set(CacheKey, articles, cacheEntryOptions);
            }

            return _mapper.Map<List<ArticlesDto>>(articles);
        }
    }
}
