using AutoMapper;
using Football247.Domain.Models.CommandModels.TagCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Tag;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class TagProfile : Profile
    {
        public TagProfile()
        {
            CreateMap<Tag, TagDto>().ReverseMap();
            CreateMap<CreateTagCommandModel, Tag>().ReverseMap();
            CreateMap<UpdateTagCommandModel, Tag>().ReverseMap();
        }
    }
}
