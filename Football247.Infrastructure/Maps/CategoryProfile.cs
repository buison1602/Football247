using AutoMapper;
using Football247.Application.Command.CategoryCmd;
using Football247.Domain.Models.CommandModels.CategoryCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Category;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile() 
        {
            // Category 
            CreateMap<Category, CategoryDto>().ReverseMap();
            CreateMap<CreateCategoryCommand, Category>().ReverseMap();
            CreateMap<UpdateCategoryCommand, Category>().ReverseMap();  
        }
    }
}
