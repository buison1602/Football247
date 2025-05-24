using AutoMapper;
using Football247.Models.DTOs.Category;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile() 
        {
            // Category 
            CreateMap<AddCategoryRequestDto, Category>().ReverseMap();
            CreateMap<UpdateCategoryRequestDto, Category>().ReverseMap();
            CreateMap<DeleteCategoryRequestDto, Category>().ReverseMap();
            CreateMap<Category, CategoryDto>().ReverseMap();
        }
    }
}
