using AutoMapper;
using Football247.Application.Command.Store.ProductCategoryCmd;
using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.ProductCategory;


namespace Football247.Infrastructure.Maps
{
    public class ProductCategoryProfile : Profile
    {
        public ProductCategoryProfile()
        {
            CreateMap<ProductCategory, ProductCategoryDto>().ReverseMap();
            CreateMap<CreateProductCategoryCommand, ProductCategory>().ReverseMap();
            CreateMap<UpdateProductCategoryCommand, ProductCategory>().ReverseMap();
        }
    }
}
