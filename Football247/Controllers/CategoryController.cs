using Football247.Authorization;
using Football247.Models.DTOs.Category;
using Football247.Services.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryService _categoryService;

        public CategoryController(ILogger<CategoryController> logger, ICategoryService categoryService)
        {
            _logger = logger;
            _categoryService = categoryService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}" );

            try
            {
                List<CategoryDto> categoryDtos = await _categoryService.GetAllAsync();

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
                CategoryDto categoryDto = await _categoryService.GetByIdAsync(id);

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
                CategoryDto categoryDto = await _categoryService.GetBySlugAsync(slug);
                if (categoryDto == null)
                {
                    return NotFound();
                }
                return Ok(categoryDto);
            } 
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{MethodBase.GetCurrentMethod()?.Name} error: {ex.Message}");
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost]
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Permissions.Categories.Create)]
        public async Task<IActionResult> Create([FromBody] AddCategoryRequestDto addCategoryRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
            {
                if (!ModelState.IsValid) return BadRequest();

                CategoryDto categoryDto = await _categoryService.CreateAsync(addCategoryRequestDto);
                if (categoryDto == null)
                {
                    return BadRequest();
                }
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
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Permissions.Categories.Edit)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                CategoryDto categoryDto = await _categoryService.UpdateAsync(id, updateCategoryRequestDto);
                if (categoryDto == null)
                {
                    return NotFound();
                }
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
        //[Authorize(Roles = "Admin")]
        [Authorize(Policy = Permissions.Categories.Delete)]
        public async Task<IActionResult> Delete(Guid id)
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");
            try
            {
                CategoryDto categoryDto = await _categoryService.DeleteAsync(id);
                if (categoryDto == null)
                {
                    return NotFound();
                }
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
