using AutoMapper;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Football247.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public CategoryController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _unitOfWork.CategoryRepository.GetAllAsync();
            if (categories == null || !categories.Any())
            {
                return NotFound();
            }

            // Map the list of Category entities to a list of CategoryDto
            LinkedList<CategoryDto> categoryDtos = _mapper.Map<LinkedList<CategoryDto>>(categories);
            return Ok(categories);
        }

        [HttpGet]
        [Route("{id:Guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var categoryDomain = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (categoryDomain == null)
            {
                return NotFound();
            }

            CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);

            return Ok(categoryDto);
        }

        [HttpGet]
        [Route("{slug}")]
        public async Task<IActionResult> GetBySlug(string slug)
        {
            var categoryDomain = await _unitOfWork.CategoryRepository.GetBySlugAsync(slug);
            if (categoryDomain == null)
            {
                return NotFound();
            }

            CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);

            return Ok(categoryDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AddCategoryRequestDto addCategoryRequestDto)
        {
            // Map DTO to Domain Model
            Category categoryDomain = _mapper.Map<Category>(addCategoryRequestDto);

            // Create the category
            categoryDomain = await _unitOfWork.CategoryRepository.CreateAsync(categoryDomain);

            // Map Domain Model back to DTO
            CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);

            return CreatedAtAction(nameof(GetById), new { id = categoryDto.Id }, categoryDto);
        }

        [HttpPut]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            var categoryDomain = _mapper.Map<Category>(updateCategoryRequestDto);
            var updatedCategory = await _unitOfWork.CategoryRepository.UpdateAsync(id, categoryDomain);

            if (updatedCategory == null)
            {
                return NotFound();
            }

            // Map Domain Model back to DTO
            CategoryDto categoryDto = _mapper.Map<CategoryDto>(updatedCategory);

            return Ok(categoryDto);
        }

        [HttpDelete]
        [Route("{id:Guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var categoryDomain = await _unitOfWork.CategoryRepository.DeleteAsync(id);

            if (categoryDomain == null)
            {
                return NotFound();
            }

            // Map Domain Model back to DTO
            CategoryDto categoryDto = _mapper.Map<CategoryDto>(categoryDomain);
            return Ok(categoryDto);
        }
    }
}
