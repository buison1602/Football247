using AutoMapper;
using Football247.Models.DTOs.Category;
using Football247.Models.DTOs.Tag;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.IService;
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
        private readonly ILogger<TagController> _logger;
        private readonly ITagService _tagService;

        public TagController(ILogger<TagController> logger, ITagService tagService)
        {
            _logger = logger;
            _tagService = tagService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation($"Start {MethodBase.GetCurrentMethod()?.Name}");

            try
            {
                List<TagDto> tagDtos = await _tagService.GetAllAsync();

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
                TagDto tagDto = await _tagService.GetBySlugAsync(slug);

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
                TagDto tagDto = await _tagService.CreateAsync(addTagRequestDto);

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
                TagDto tagDto = await _tagService.UpdateAsync(id, updateTagRequestDto);

                return Ok(tagDto);
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
                TagDto tagDto = await _tagService.DeleteAsync(id);
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
