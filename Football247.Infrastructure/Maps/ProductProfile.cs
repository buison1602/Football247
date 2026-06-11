using AutoMapper;
using Football247.Application.Command.Store.ProductCmd;
using Football247.Domain.Entities.Stores;
using Football247.Domain.Models.EntityModels.DTOs.Product;


namespace Football247.Infrastructure.Maps
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDetailDto>().ReverseMap();
            CreateMap<CreateProductCommand, Product>().ReverseMap();
            CreateMap<UpdateProductCommand, Product>().ReverseMap();
        }
    }
}
