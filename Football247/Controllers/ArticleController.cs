using AutoMapper;
using Azure.Core;
using Football247.Authorization;
using Football247.Models.DTOs.Article;
using Football247.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly ILogger<ArticleController> _logger;

        private readonly IArticleService _articleService;

        public ArticleController(ILogger<ArticleController> logger, IArticleService articleService)
        {
            _logger = logger;
            _articleService = articleService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
            {
                List<ArticlesDto> articlesDtos = await _articleService.GetAllAsync();
                if (articlesDtos == null) return NotFound();

                return Ok(articlesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("Get5Articles")]
        public async Task<IActionResult> Get5Articles([FromQuery] int limit = 5, 
            [FromQuery] string sort = "newest")
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
            {
                List<ArticlesDto> articlesDtos = await _articleService.Get5ArticlesAsync();
                if (articlesDtos == null) return NotFound();
                return Ok(articlesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("{categorySlug}/articles")]
        [Route("{categorySlug}/page/{page:int}")]
        public async Task<IActionResult> GetByCategory(string categorySlug, int page = 1)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with categorySlug: {categorySlug}, page: {page}");
            try
            {
                List<ArticlesDto> articlesDtos = await _articleService.GetByCategoryAsync(categorySlug, page);
                if (articlesDtos == null) return NotFound();
                return Ok(articlesDtos);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("tag/{tagSlug}/articles")]
        [Route("tag/{tagSlug}/page/{page:int}")]
        public async Task<IActionResult> GetByTag(string tagSlug, int page = 1)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with tagSlug: {tagSlug}, page: {page}");   
            try
            {
                List<ArticlesDto> articlesDtos = await _articleService.GetByTagAsync(tagSlug, page);
                if (articlesDtos == null) return NotFound();
                return Ok(articlesDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet]
        [Route("{articleSlug}")]
        public async Task<IActionResult> GetArticleBySlug(string articleSlug)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with articleSlug: {articleSlug}");

            try
            {
                var articleDto = await _articleService.GetBySlugAsync(articleSlug);
                if (articleDto == null) return NotFound();
                return Ok(articleDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Permissions.Articles.Create)]
        public async Task<IActionResult> Create([FromForm] AddArticleRequestDto addArticleRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                var articleDto = await _articleService.CreateAsync(addArticleRequestDto);

                return CreatedAtAction(
                    actionName: nameof(GetArticleBySlug),
                    routeValues: new { articleSlug = articleDto.Slug },   
                    value: articleDto
                );
            }
            catch (InvalidOperationException ex)   
            {
                _logger.LogWarning(ex, "Create Article failed");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut]
        [Route("{id:guid}")]
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Permissions.Articles.Edit)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleRequestDto updateArticleRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with id: {id}");
            try
            {
                var articleDto = await _articleService.UpdateAsync(id, updateArticleRequestDto);
                return Ok(articleDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Update Article failed");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        [Route("{id:guid}")]
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Permissions.Articles.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name} with id: {id}");
            try
            {
                var articleDto = await _articleService.DeleteAsync(id);
                return Ok(articleDto);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Update Article failed");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }
    }
}
