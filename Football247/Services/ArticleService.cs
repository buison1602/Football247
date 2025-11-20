using AutoMapper;
using Football247.Models.DTOs.Article;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.Caching;
using Football247.Services.IService;

namespace Football247.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCacheService;
        private const string CacheKey = "articles";

        public ArticleService(IUnitOfWork unitOfWork, IMapper mapper, IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _redisCacheService = redisCacheService;
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

            await _redisCacheService.RemoveDataAsync(CacheKey);

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
            await _redisCacheService.RemoveDataAsync(CacheKey);
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

            await _redisCacheService.RemoveDataAsync(CacheKey);
            return _mapper.Map<ArticleDto>(articleDomain);
        }


        public async Task<List<ArticlesDto>> GetByCategoryAsync(string categorySlug, int page)
        {
            string newCacheKey = $"articles_{categorySlug}_{page}";
            List<Article>? articles = await _redisCacheService.GetDataAsync<List<Article>>(newCacheKey);

            if (articles != null)
            {
                return _mapper.Map<List<ArticlesDto>>(articles);
            }
            articles = await _unitOfWork.ArticleRepository.GetByCategoryAsync(categorySlug, page);
            articles ??= new List<Article>();

            await _redisCacheService.SetDataAsync(newCacheKey, articles);

            return _mapper.Map<List<ArticlesDto>>(articles);
        }


        public async Task<List<ArticlesDto>> GetByTagAsync(string tagSlug, int page)
        {
            string newCacheKey = $"articles_{tagSlug}_{page}";
            List<Article>? articles = await _redisCacheService.GetDataAsync<List<Article>>(newCacheKey);

            if (articles != null)
            {
                return _mapper.Map<List<ArticlesDto>>(articles);
            }
            
            articles = await _unitOfWork.ArticleRepository.GetByTagAsync(tagSlug, page);
            articles ??= new List<Article>();

            await _redisCacheService.SetDataAsync(newCacheKey, articles);

            return _mapper.Map<List<ArticlesDto>>(articles);
        }


        public async Task<ArticleDto> GetBySlugAsync(string articleSlug)
        {
            string newCacheKey = $"articles_{articleSlug}";
            Article? articleDomain = await _redisCacheService.GetDataAsync<Article>(newCacheKey);

            if (articleDomain != null)
            {
                return _mapper.Map<ArticleDto>(articleDomain);
            }
            articleDomain = await _unitOfWork.ArticleRepository.GetBySlugAsync(articleSlug);
            articleDomain ??= new Article();

            await _redisCacheService.SetDataAsync(newCacheKey, articleDomain);

            return _mapper.Map<ArticleDto>(articleDomain);
        }


        public async Task<List<ArticlesDto>> Get5ArticlesAsync()
        {
            string newCacheKey = $"5NewArticles";
            List<Article>? articles = await _redisCacheService.GetDataAsync<List<Article>>(newCacheKey);

            if (articles != null)
            {
                return _mapper.Map<List<ArticlesDto>>(articles);
            }
            articles = await _unitOfWork.ArticleRepository.Get5ArticlesAsync();
            articles ??= new List<Article>();

            await _redisCacheService.SetDataAsync(newCacheKey, articles);

            return _mapper.Map<List<ArticlesDto>>(articles);
        }


        public async Task<List<ArticlesDto>> GetAllAsync()
        {
            List<Article>? articles = await _redisCacheService.GetDataAsync<List<Article>>(CacheKey);

            if (articles != null)
            {
                return _mapper.Map<List<ArticlesDto>>(articles);
            }
            articles = await _unitOfWork.ArticleRepository.GetAllAsync();
            articles ??= new List<Article>();

            await _redisCacheService.SetDataAsync(CacheKey, articles);

            return _mapper.Map<List<ArticlesDto>>(articles);
        }
    }
}
