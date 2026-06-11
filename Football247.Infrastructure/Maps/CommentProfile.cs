using AutoMapper;
using Football247.Domain.Models.CommandModels.CommentCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Comment;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<CreateCommentCommandModel, Comment>().ReverseMap();
            CreateMap<CommentDto, Comment>().ReverseMap();
        }
    }
}
