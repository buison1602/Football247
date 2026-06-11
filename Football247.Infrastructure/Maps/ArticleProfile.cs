using AutoMapper;
using Football247.Application.Command.ArticleCmd; 
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile()
        {
            CreateMap<CreateArticleCommand, Article>()
                .AfterMap((src, dest) => {
                    if (src.TagIds != null)
                    {
                        dest.ArticleTags = src.TagIds
                            .Select(tagId => new ArticleTag { TagId = tagId, ArticleId = dest.Id })
                            .ToList();
                    }
                    else
                    {
                        dest.ArticleTags = new List<ArticleTag>();
                    }
                });

            CreateMap<UpdateArticleCommand, Article>();//.IgnoreAllNonExisting();


            CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.BgrImage,
                           opt => opt.MapFrom(src => src.BgrImage))
                .ForMember(dest => dest.CreatedFullName,
                           opt => opt.MapFrom(src => src.CreatedFullName))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(dest => dest.Tags,
                           opt => opt.MapFrom(src => (src.ArticleTags != null)
                                                     ? src.ArticleTags
                                                         .Where(at => at.Tag != null)
                                                         .Select(at => at.Tag)
                                                         .ToList()
                                                     : new List<Tag>()));

            CreateMap<Article, ArticlesDto>()
               .ForMember(dest => dest.BgrImage,
                    opt => opt.MapFrom(src => src.BgrImage))
               .ForMember(dest => dest.Tags,
                    opt => opt.MapFrom(src => src.ArticleTags != null
                        ? src.ArticleTags.Select(at => at.Tag.Name).ToList()
                        : new List<string>()));
        }
    }
}