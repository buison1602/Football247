using AutoMapper;
using Football247.Application.Command.TeamCmd;
using Football247.Domain.Entities;
using Football247.Domain.Models.EntityModels.DTOs.Team;

namespace Football247.Mappings
{
    public class TeamProfile : Profile
    {
        public TeamProfile()
        {
            CreateMap<CreateTeamCommand, Team>();
            CreateMap<UpdateTeamCommand, Team>();
            CreateMap<Team, TeamDto>();
        }
    }
}
