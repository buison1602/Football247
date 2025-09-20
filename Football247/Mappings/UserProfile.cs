using AutoMapper;
using Football247.Models.DTOs.User;
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
                    opt => opt.MapFrom(src => Guid.Parse(src.Id)) 
                );
        }
    }
}
