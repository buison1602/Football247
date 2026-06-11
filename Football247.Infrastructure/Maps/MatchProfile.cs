using AutoMapper;
using Football247.Domain.Entities;
using Football247.Domain.Models.EntityModels.DTOs.Match;

namespace Football247.Infrastructure.Maps
{
    public class MatchProfile : Profile
    {
        public MatchProfile()
        {
            // Match -> MatchFixtureDto
            CreateMap<Match, MatchFixtureDto>()
                .ForMember(
                    dest => dest.HomeTeam,
                    opt => opt.MapFrom(src => new TeamInMatchDto
                    {
                        ExternalId = src.HomeTeamExternalId,
                        Name = src.HomeTeamName,
                        ShortName = src.HomeTeamShortName,
                        Crest = src.HomeTeamCrest
                    }))
                .ForMember(
                    dest => dest.AwayTeam,
                    opt => opt.MapFrom(src => new TeamInMatchDto
                    {
                        ExternalId = src.AwayTeamExternalId,
                        Name = src.AwayTeamName,
                        ShortName = src.AwayTeamShortName,
                        Crest = src.AwayTeamCrest
                    }))
                .ReverseMap()
                // MatchFixtureDto -> Match
                .ForMember(
                    dest => dest.HomeTeamExternalId,
                    opt => opt.MapFrom(src => src.HomeTeam.ExternalId))
                .ForMember(
                    dest => dest.HomeTeamName,
                    opt => opt.MapFrom(src => src.HomeTeam.Name))
                .ForMember(
                    dest => dest.HomeTeamShortName,
                    opt => opt.MapFrom(src => src.HomeTeam.ShortName))
                .ForMember(
                    dest => dest.HomeTeamCrest,
                    opt => opt.MapFrom(src => src.HomeTeam.Crest))
                .ForMember(
                    dest => dest.AwayTeamExternalId,
                    opt => opt.MapFrom(src => src.AwayTeam.ExternalId))
                .ForMember(
                    dest => dest.AwayTeamName,
                    opt => opt.MapFrom(src => src.AwayTeam.Name))
                .ForMember(
                    dest => dest.AwayTeamShortName,
                    opt => opt.MapFrom(src => src.AwayTeam.ShortName))
                .ForMember(
                    dest => dest.AwayTeamCrest,
                    opt => opt.MapFrom(src => src.AwayTeam.Crest))
                // Các property computed trong DTO không cần map ngược
                .ForSourceMember(src => src.MatchDateLabel, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.MatchTimeLabel, opt => opt.DoNotValidate())
                .ForSourceMember(src => src.IsFinished, opt => opt.DoNotValidate());
        }
    }
}