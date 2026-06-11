using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Image;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class ImageProfile : Profile
    {
        public ImageProfile()
        {
            CreateMap<Image, ImageDto>().ReverseMap();
        }
    }
}
