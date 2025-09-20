using Football247.Models.DTOs.Tag;

namespace Football247.Services.IService
{
    public interface ITagService
    {
        Task<List<TagDto>> GetAllAsync();
        Task<TagDto> GetBySlugAsync(string slug);
        Task<TagDto> CreateAsync(AddTagRequestDto addTagRequestDto);
        Task<TagDto?> UpdateAsync(Guid id, UpdateTagRequestDto updateTagRequestDto);
        Task<TagDto?> DeleteAsync(Guid id);
    }
}
