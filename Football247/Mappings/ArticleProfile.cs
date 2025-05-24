using AutoMapper;
using Football247.Models.DTOs.Article;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile() 
        {
            CreateMap<AddArticleRequestDto, Article>().ReverseMap();
            CreateMap<UpdateArticleRequestDto, Article>().ReverseMap();
            CreateMap<DeleteArticleRequestDto, Article>().ReverseMap();

            //CreateMap<Article, ArticleDto>().ReverseMap();
            // Article -> ArticleDto
            CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.CreatorName,
                           opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : null))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Tags,
                           opt => opt.MapFrom(src => src.ArticleTags.Select(at => at.Tag.Name).ToList()));
        }
    }
}
