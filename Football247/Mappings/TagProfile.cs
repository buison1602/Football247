using AutoMapper;
using Football247.Models.DTOs.Category;
using Football247.Models.DTOs.Tag;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<AddTagRequestDto, Tag>().ReverseMap();
            CreateMap<UpdateTagRequestDto, Tag>().ReverseMap();
            CreateMap<Tag, TagDto>().ReverseMap();
        }
    }
}
