using AutoMapper;
using Football247.Domain.Entities;
using Football247.Domain.Models.EntityModels.DTOs.Standing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Infrastructure.Maps
{
    public class StandingProfile : Profile
    {
        public StandingProfile()
        {
            CreateMap<Standing, StandingDto>().ReverseMap();
        }
    }
}
