using AutoMapper;
using Football247.Models.DTOs.Article;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<ArticleController> _logger;
        private const string CacheKey = "articles";

        public ArticleController(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, ILogger<ArticleController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
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
                        return NotFound();
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                    _memoryCache.Set(CacheKey, articles, cacheEntryOptions);
                }

                List<ArticlesDto> articlesDtos = _mapper.Map<List<ArticlesDto>>(articles);

                return Ok(articlesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("{categorySlug}")]
        [Route("{categorySlug}/page/{page:int}")]
        public async Task<IActionResult> GetByCategory(string categorySlug, int page = 1)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with categorySlug: {categorySlug}, page: {page}");
            try
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
                        _logger.LogWarning($"No articles found for categorySlug: {categorySlug}, page: {page}");
                        return NotFound();
                    }
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                    _memoryCache.Set(NewCacheKey, articles, cacheEntryOptions);
                }

                List<ArticlesDto> articlesDtos = _mapper.Map<List<ArticlesDto>>(articles);
                return Ok(articlesDtos);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("tag/{tagSlug}")]
        [Route("tag/{tagSlug}/page/{page:int}")]
        public async Task<IActionResult> GetByTag(string tagSlug, int page = 1)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with tagSlug: {tagSlug}, page: {page}");   
            try
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
                        _logger.LogWarning($"No articles found for tagSlug: {tagSlug}, page: {page}");
                        return NotFound();
                    }

                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                    _memoryCache.Set(NewCacheKey, articles, cacheEntryOptions);
                }

                List<ArticlesDto> articlesDtos = _mapper.Map<List<ArticlesDto>>(articles);
                return Ok(articlesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("{categorySlug}/{articleSlug}")]
        public async Task<IActionResult> GetBySlug(string categorySlug, string articleSlug)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with categorySlug: {categorySlug}, articleSlug: {articleSlug}");
            try
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
                        _logger.LogWarning($"No article found for categorySlug: {categorySlug}, articleSlug: {articleSlug}");
                        return NotFound();
                    }
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromHours(2));
                    _memoryCache.Set(NewCacheKey, articleDomain, cacheEntryOptions);
                }

                var articleDto = _mapper.Map<ArticleDto>(articleDomain);
                return Ok(articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddArticleRequestDto addArticleRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                var articleDomain = _mapper.Map<Article>(addArticleRequestDto);
                articleDomain = await _unitOfWork.ArticleRepository.CreateAsync(articleDomain);
                if (articleDomain == null)
                {
                    return BadRequest();
                }
                _memoryCache.Remove(CacheKey);
                ArticleDto articleDto = _mapper.Map<ArticleDto>(articleDomain);

                return CreatedAtAction(nameof(GetBySlug), new { categorySlug = articleDomain.Category.Slug, articleSlug = articleDto.Slug }, articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleRequestDto updateArticleRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with id: {id}");
            try
            {
                var articleDomain = _mapper.Map<Article>(updateArticleRequestDto);
                articleDomain = await _unitOfWork.ArticleRepository.UpdateAsync(id, articleDomain);
                if (articleDomain == null)
                {
                    return NotFound();
                }
                _memoryCache.Remove(CacheKey);
                ArticleDto articleDto = _mapper.Map<ArticleDto>(articleDomain);
                return Ok(articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with id: {id}");
            try
            {
                var articleDomain = await _unitOfWork.ArticleRepository.DeleteAsync(id);
                if (articleDomain == null)
                {
                    return NotFound();
                }
                _memoryCache.Remove(CacheKey);
                ArticleDto articleDto = _mapper.Map<ArticleDto>(articleDomain);
                return Ok(articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
