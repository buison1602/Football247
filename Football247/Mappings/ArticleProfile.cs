using AutoMapper;
using Football247.Models.DTOs.Article;
using Football247.Models.Entities;

namespace Football247.Mappings
{
    public class ArticleProfile : Profile
    {
        public ArticleProfile() 
        {
            CreateMap<AddArticleRequestDto, Article>()
            //.ForMember(dest => dest.Id, opt => opt.MapFrom(src => Guid.NewGuid()))
            .AfterMap((src, dest) => {
                if (src.TagIds != null)
                {
                    // Chuyển đổi TagIds thành danh sách ArticleTag
                    dest.ArticleTags = src.TagIds
                        .Select(tagId => new ArticleTag { TagId = tagId, ArticleId = dest.Id })
                        .ToList();
                }
                else
                {
                    dest.ArticleTags = new List<ArticleTag>();
                }
            });

            CreateMap<UpdateArticleRequestDto, Article>().ReverseMap();
            CreateMap<DeleteArticleRequestDto, Article>().ReverseMap();

            CreateMap<Article, ArticleDto>()
                .ForMember(dest => dest.CreatorName,
                           opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : null))
                .ForMember(dest => dest.CategoryName,
                           opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                // dest.Tags bây giờ là List<TagDto>
                .ForMember(dest => dest.Tags, 
                           opt => opt.MapFrom(src => (src.ArticleTags != null)
                                                     ? src.ArticleTags
                                                         .Where(at => at.Tag != null) // Chỉ lấy các ArticleTag có Tag không null
                                                         .Select(at => at.Tag)        // Lấy ra đối tượng Tag
                                                         .ToList()                    // Kết quả là List<Tag>
                                                     : new List<Tag>()));       // Nếu ArticleTags là null, trả về List<Tag> rỗng
                                                                                // AutoMapper sẽ tự động áp dụng map Tag -> TagDto cho từng item

            // Used when multiple articles need to be returned
            CreateMap<Article, ArticlesDto>()
               .ForMember(dest => dest.BgrImg,
                    opt => opt.MapFrom(src => src.Images != null && src.Images.Any()
                        ? src.Images.OrderBy(img => img.DisplayOrder).FirstOrDefault()!.ImageUrl 
                        : null))
               .ForMember(dest => dest.Tags, 
                    opt => opt.MapFrom(src => src.ArticleTags != null
                        ? src.ArticleTags.Select(at => at.Tag.Name).ToList() 
                        : new List<string>()));
        }
    }
}
