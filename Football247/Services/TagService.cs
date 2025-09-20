using AutoMapper;
using Football247.Controllers;
using Football247.Models.DTOs.Category;
using Football247.Models.DTOs.Tag;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.IService;
using Microsoft.Extensions.Caching.Memory;

namespace Football247.Services
{
    public class TagService : ITagService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private readonly ILogger<TagController> _logger;
        private const string CacheKey = "tags";

        public TagService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache, ILogger<TagController> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
            _logger = logger;
        }

        public async Task<TagDto> CreateAsync(AddTagRequestDto addTagRequestDto)
        {
            Tag? tagDomain;

            tagDomain = await _unitOfWork.TagRepository.GetByNameAsync(addTagRequestDto.Name);
            if (tagDomain != null)
            {
                throw new InvalidOperationException($"A tag with the name '{addTagRequestDto.Name}' already exists.");
            }

            tagDomain = await _unitOfWork.TagRepository.GetBySlugAsync(addTagRequestDto.Slug);
            if (tagDomain != null)
            {
                throw new InvalidOperationException($"A tag with the slug '{addTagRequestDto.Slug}' already exists.");
            }

            tagDomain = _mapper.Map<Tag>(addTagRequestDto);
            tagDomain = await _unitOfWork.TagRepository.CreateAsync(tagDomain);
            if (tagDomain == null)
            {
                throw new InvalidOperationException("Failed to create the tag in the database.");
            }
            _memoryCache.Remove(CacheKey);
            return _mapper.Map<TagDto>(tagDomain);
        }

        public async Task<TagDto?> DeleteAsync(Guid id)
        {
            Tag? tagDomain;

            tagDomain = await _unitOfWork.TagRepository.GetByIdAsync(id);
            if (tagDomain == null)
            {
                throw new InvalidOperationException("Tag don't exist");
            }

            tagDomain = await _unitOfWork.TagRepository.DeleteAsync(id);
            if (tagDomain == null)
            {
                return null;
            }
            _memoryCache.Remove(CacheKey);
            return _mapper.Map<TagDto>(tagDomain);
        }

        public async Task<List<TagDto>> GetAllAsync()
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
                    tags = new List<Tag>();
                }

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                _memoryCache.Set(CacheKey, tags, cacheEntryOptions);
            }
            return _mapper.Map<List<TagDto>>(tags);
        }

        public async Task<TagDto> GetBySlugAsync(string slug)
        {
            Tag? TagDomain;

            if (_memoryCache.TryGetValue(CacheKey, out List<Tag>? data))
            {
                TagDomain = data?.FirstOrDefault(c => c.Slug == slug);
                if (TagDomain != null)
                {
                    return _mapper.Map<TagDto>(TagDomain);
                }
            }
            TagDomain = await _unitOfWork.TagRepository.GetBySlugAsync(slug);
            if (TagDomain == null)
            {
                return null;
            }
            return _mapper.Map<TagDto>(TagDomain);
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
            _memoryCache.Remove(CacheKey);
            return _mapper.Map<TagDto>(updatedTag);
        }
    }
}
