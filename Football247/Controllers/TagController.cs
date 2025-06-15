using AutoMapper;
using Football247.Models.DTOs.Category;
using Football247.Models.DTOs.Tag;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TagController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TagController> _logger;
        private const string CacheKey = "tags";

        public TagController(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, ILogger<TagController> logger)
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
                List<Tag>? tags;

                if (_memoryCache.TryGetValue(CacheKey, out List<Tag>? data))
                {
                    tags = data;
                }
                else
                {
                    tags = await _unitOfWork.TagRepository.GetAllAsync();
                    if (tags == null || !tags.Any())
                    {
                        return NotFound();
                    }

                    // Set cache options
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                    _memoryCache.Set(CacheKey, tags, cacheEntryOptions);
                }
                // Map the list of Tag entities to a list of TagDto
                List<TagDto> tagDtos = _mapper.Map<List<TagDto>>(tags);

                return Ok(tagDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet]
        [Route("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
            {
                Tag? TagDomain;

                if (_memoryCache.TryGetValue(CacheKey, out List<Tag>? data))
                {
                    TagDomain = data?.FirstOrDefault(c => c.Slug == slug);
                    if (TagDomain != null)
                    {
                        return Ok(_mapper.Map<TagDto>(TagDomain));
                    }
                }
                TagDomain = await _unitOfWork.TagRepository.GetBySlugAsync(slug);
                if (TagDomain == null)
                {
                    return NotFound();
                }
                TagDto tagDto = _mapper.Map<TagDto>(TagDomain);

                return Ok(tagDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AddTagRequestDto addTagRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
            {
                Tag tagDomain = _mapper.Map<Tag>(addTagRequestDto);
                tagDomain = await _unitOfWork.TagRepository.CreateAsync(tagDomain);
                if (tagDomain == null)
                {
                    return BadRequest();
                }
                _memoryCache.Remove(CacheKey);
                TagDto tagDto = _mapper.Map<TagDto>(tagDomain);

                return CreatedAtAction(nameof(GetBySlug), new { slug = tagDto.Slug }, tagDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTagRequestDto updateTagRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                var tagDomain = _mapper.Map<Tag>(updateTagRequestDto);
                var updatedTag = await _unitOfWork.TagRepository.UpdateAsync(id, tagDomain);
                if (updatedTag == null)
                {
                    return NotFound();
                }
                _memoryCache.Remove(CacheKey);
                TagDto tagDto = _mapper.Map<TagDto>(updatedTag);

                return Ok(tagDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }

    }
}
