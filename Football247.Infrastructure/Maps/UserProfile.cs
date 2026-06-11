using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.User;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(
                    dest => dest.Id, 
                    opt => opt.MapFrom(src => src.Id) 
                );
        }
    }
}
