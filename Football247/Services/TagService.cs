using AutoMapper;
using Football247.Controllers;
using Football247.Models.DTOs.Category;
using Football247.Models.DTOs.Tag;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.Caching;
using Football247.Services.IService;
using Microsoft.Extensions.Caching.Memory;

namespace Football247.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IRedisCacheService _redisCacheService;
        private readonly ILogger<TagController> _logger;
        private const string CacheKey = "tags";

        public TagService(IUnitOfWork unitOfWork, 
            IMapper mapper, 
            ILogger<TagController> logger, 
            IRedisCacheService redisCacheService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _redisCacheService = redisCacheService;
        }

        public async Task<TagDto> CreateAsync(AddTagRequestDto addTagRequestDto)
        {
            TagDto tagDto = await _unitOfWork.TagRepository.GetByNameAsync(addTagRequestDto.Name);
            if (tagDto != null)
            {
                throw new InvalidOperationException($"A tag with the name '{addTagRequestDto.Name}' already exists.");
            }

            tagDto = await _unitOfWork.TagRepository.GetBySlugAsync(addTagRequestDto.Slug);
            if (tagDto != null)
            {
                throw new InvalidOperationException($"A tag with the slug '{addTagRequestDto.Slug}' already exists.");
            }

            Tag? tagDomain = _mapper.Map<Tag>(addTagRequestDto);
            tagDomain = await _unitOfWork.TagRepository.CreateAsync(tagDomain);
            if (tagDomain == null)
            {
                throw new InvalidOperationException("Failed to create the tag in the database.");
            }

            await _redisCacheService.RemoveDataAsync(CacheKey);

            return _mapper.Map<TagDto>(tagDomain);
        }

        public async Task<TagDto?> DeleteAsync(Guid id)
        {
            Tag? tagDomain = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tagDomain == null)
            {
                throw new InvalidOperationException("Tag don't exist");
            }

            tagDomain = await _unitOfWork.TagRepository.DeleteAsync(id);
            if (tagDomain == null)
            {
                return null;
            }
            await _redisCacheService.RemoveDataAsync(CacheKey);
            return _mapper.Map<TagDto>(tagDomain);
        }

        public async Task<List<TagDto>> GetAllAsync()
        {
            List<Tag>? tags = await _redisCacheService.GetDataAsync<List<Tag>>(CacheKey);

            if (tags != null)
            {
                return _mapper.Map<List<TagDto>>(tags);
            }

            tags = await _unitOfWork.TagRepository.GetAllAsync();
            tags ??= new List<Tag>();

            await _redisCacheService.SetDataAsync(CacheKey, tags);
            return _mapper.Map<List<TagDto>>(tags);
        }

        public async Task<TagDto> GetBySlugAsync(string slug)
        {
            String newCacheKey = $"{CacheKey}-{slug}";
            TagDto? tagDomain = await _redisCacheService.GetDataAsync<TagDto>(newCacheKey);

            if (tagDomain != null)
            {
                return tagDomain;
            }

            tagDomain = await _unitOfWork.TagRepository.GetBySlugAsync(slug);
            
            await _redisCacheService.SetDataAsync(newCacheKey, tagDomain);

            return tagDomain;
        }

        public async Task<TagDto?> UpdateAsync(Guid id, UpdateTagRequestDto updateTagRequestDto)
        {
            Tag? tagDomain;

            tagDomain = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tagDomain == null)
            {
                throw new InvalidOperationException("Tag don't exist");
            }

            tagDomain = _mapper.Map<Tag>(updateTagRequestDto);
            var updatedTag = await _unitOfWork.TagRepository.UpdateAsync(id, tagDomain);
            if (updatedTag == null)
            {
                return null;
            }

            await _redisCacheService.RemoveDataAsync(CacheKey);

            return _mapper.Map<TagDto>(updatedTag);
        }
    }
}
