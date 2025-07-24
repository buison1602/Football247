using AutoMapper;
using Football247.Models.DTOs.Comment;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class CommentProfile : Profile
    {
        public CommentProfile()
        {
            CreateMap<AddCommentRequestDto, Comment>().ReverseMap();
            CreateMap<CommentDto, Comment>().ReverseMap();
        }
    }
}
