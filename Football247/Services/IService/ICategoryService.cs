using Football247.Models.DTOs.Category;

namespace Football247.Services.IService
{
    public interface ICategoryService
    {
        Task<List<CategoryDto>> GetAllAsync();
        Task<CategoryDto?> GetByIdAsync(Guid id);
        Task<CategoryDto?> GetBySlugAsync(string slug);
        Task<CategoryDto> CreateAsync(AddCategoryRequestDto addCategoryRequestDto);
        Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequestDto updateCategoryRequestDto);
        Task<CategoryDto?> DeleteAsync(Guid id);
    }
}
