using AutoMapper;
using Football247.Controllers;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Services.IService;
using Microsoft.Extensions.Caching.Memory;

namespace Football247.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;
        private const string CacheKey = "categories";
        
        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IMemoryCache memoryCache)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }


        public async Task<CategoryDto> CreateAsync(AddCategoryRequestDto addCategoryRequestDto)
        {
            Category? categoryDomain;
            
            categoryDomain = await _unitOfWork.CategoryRepository.GetByNameAsync(addCategoryRequestDto.Name);
            if (categoryDomain != null)
            {
                throw new InvalidOperationException($"A category with the name '{addCategoryRequestDto.Name}' already exists.");
            }

            categoryDomain = await _unitOfWork.CategoryRepository.GetBySlugAsync(addCategoryRequestDto.Slug);
            if (categoryDomain != null) 
            {
                throw new InvalidOperationException($"A category with the slug '{addCategoryRequestDto.Slug}' already exists.");
            }

            categoryDomain = _mapper.Map<Category>(addCategoryRequestDto);
            categoryDomain = await _unitOfWork.CategoryRepository.CreateAsync(categoryDomain);
            if (categoryDomain == null)
            {
                throw new InvalidOperationException("Failed to create the category in the database.");
            }

            await _unitOfWork.SaveAsync();

            _memoryCache.Remove(CacheKey);
            return _mapper.Map<CategoryDto>(categoryDomain);
        }


        public async Task<CategoryDto?> DeleteAsync(Guid id)
        {
            Category? categoryDomain;

            categoryDomain = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (categoryDomain == null)
            {
                throw new InvalidOperationException("Category don't exist");
            }

            categoryDomain = await _unitOfWork.CategoryRepository.DeleteAsync(id);
            if (categoryDomain == null)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();

            _memoryCache.Remove(CacheKey);
            return _mapper.Map<CategoryDto>(categoryDomain);
        }


        public async Task<List<CategoryDto>> GetAllAsync()
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
                    categories = new List<Category>();
                }

                // Set cache options
                var cacheEntryOptions = new MemoryCacheEntryOptions().SetSlidingExpiration(TimeSpan.FromDays(1));
                _memoryCache.Set(CacheKey, categories, cacheEntryOptions);
            }
            // Map the list of Category entities to a list of CategoryDto
            return _mapper.Map<List<CategoryDto>>(categories);
        }


        public async Task<CategoryDto?> GetByIdAsync(Guid id)
        {
            Category? categoryDomain;

            if (_memoryCache.TryGetValue(CacheKey, out List<Category>? data))
            {
                categoryDomain = data?.FirstOrDefault(c => c.Id == id);
                if (categoryDomain != null)
                {
                    return _mapper.Map<CategoryDto>(categoryDomain);
                }
            }

            categoryDomain = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (categoryDomain == null)
            {
                return null;
            }
            return _mapper.Map<CategoryDto>(categoryDomain);
        }


        public async Task<CategoryDto?> GetBySlugAsync(string slug)
        {
            Category? categoryDomain;

            if (_memoryCache.TryGetValue(CacheKey, out List<Category>? data))
            {
                categoryDomain = data?.FirstOrDefault(c => c.Slug == slug);
                if (categoryDomain != null)
                {
                    return _mapper.Map<CategoryDto>(categoryDomain);
                }
            }
            categoryDomain = await _unitOfWork.CategoryRepository.GetBySlugAsync(slug);
            if (categoryDomain == null)
            {
                return null;
            }
            return _mapper.Map<CategoryDto>(categoryDomain);
        }


        public async Task<CategoryDto?> UpdateAsync(Guid id, UpdateCategoryRequestDto updateCategoryRequestDto)
        {
            Category? categoryDomain;

            categoryDomain = await _unitOfWork.CategoryRepository.GetByIdAsync(id);
            if (categoryDomain == null)
            {
                throw new InvalidOperationException("Category don't exist");
            }

            categoryDomain = _mapper.Map<Category>(updateCategoryRequestDto);
            var updatedCategory = await _unitOfWork.CategoryRepository.UpdateAsync(id, categoryDomain);
            if (updatedCategory == null)
            {
                return null;
            }

            await _unitOfWork.SaveAsync();

            _memoryCache.Remove(CacheKey);
            return _mapper.Map<CategoryDto>(updatedCategory);
        }
    }
}
