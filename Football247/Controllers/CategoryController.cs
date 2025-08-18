using AutoMapper;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
using System.Reflection;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<CategoryController> _logger;
        private const string CacheKey = "categories";

        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, ILogger<CategoryController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _logger = logger;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}" );

            try
            {
                List<Category>? categories;

                if (_memoryCache.TryGetValue(CacheKey, out List<Category>? data))
                {
                    categories = data;
                }
                else
                {
                    categories = await _unitOfWork.CategoryRepository.GetAllAsync();
                    if (categories == null || !categories.Any())
                    {
                        return NotFound();
                    }

                    // Set cache options
                    var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                    _memoryCache.Set(CacheKey, categories, cacheEntryOptions);
                }
                // Map the list of Category entities to a list of CategoryDto
                List<CategoryDto> categoryDtos = _mapper.Map<List<CategoryDto>>(categories);

                return Ok(categoryDtos);
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            } 
        }


        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                Category? categoryDomain;

                if (_memoryCache.TryGetValue(CacheKey, out List<Category>? data))
                {
                    categoryDomain = data?.FirstOrDefault(c => c.Id == id);
                    if (categoryDomain != null)
                    {
                        return Ok(_mapper.Map<CategoryDto>(categoryDomain));
                    }
                }
                categoryDomain = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
                if (categoryDomain == null)
                {
                    return NotFound();
                }
                CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);

                return Ok(categoryDto);
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
                Category? categoryDomain;

                if (_memoryCache.TryGetValue(CacheKey, out List<Category>? data))
                {
                    categoryDomain = data?.FirstOrDefault(c => c.Slug == slug);
                    if (categoryDomain != null)
                    {
                        return Ok(_mapper.Map<CategoryDto>(categoryDomain));
                    }
                }
                categoryDomain = await _unitOfWork.CategoryRepository.GetBySlugAsync(slug);
                if (categoryDomain == null)
                {
                    return NotFound();
                }
                CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);

                return Ok(categoryDto);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([FromBody] AddCategoryRequestDto addCategoryRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
            {
                Category categoryDomain = _mapper.Map<Category>(addCategoryRequestDto);
                categoryDomain = await _unitOfWork.CategoryRepository.CreateAsync(categoryDomain);
                if (categoryDomain == null)
                {
                    return BadRequest();
                }
                _memoryCache.Remove(CacheKey);
                CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);

                return CreatedAtAction(nameof(GetBySlug), new { slug = categoryDto.Slug }, categoryDto);
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
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                var categoryDomain = _mapper.Map<Category>(updateCategoryRequestDto);
                var updatedCategory = await _unitOfWork.CategoryRepository.UpdateAsync(id, categoryDomain);
                if (updatedCategory == null)
                {
                    return NotFound();
                }
                _memoryCache.Remove(CacheKey);
                CategoryDto categoryDto = _mapper.Map<CategoryDto>(updatedCategory);

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete]
        [Route("{id:Guid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                var categoryDomain = await _unitOfWork.CategoryRepository.DeleteAsync(id);
                if (categoryDomain == null)
                {
                    return NotFound();
                }
                _memoryCache.Remove(CacheKey);
                CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);

                return Ok(categoryDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
            
        }
    }
}
