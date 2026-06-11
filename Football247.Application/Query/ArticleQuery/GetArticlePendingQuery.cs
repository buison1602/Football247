using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;


namespace Football247.Application.Query.ArticleQuery
{
    public class GetArticlePendingQuery : IRequest<MethodResult<List<ArticlesDto>>>
    {
    }
    
    public class GetArticlePendingQueryHandler : IRequestHandler<GetArticlePendingQuery, MethodResult<List<ArticlesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetArticlePendingQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<ArticlesDto>>> Handle(GetArticlePendingQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<ArticlesDto>>();

            var articles = await _unitOfWork.ArticleRepository.ReadQueryable
                .Where(a => a.Status == EnumStatusArticle.Pending)
                .Select(a => new ArticlesDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    BgrImage = a.BgrImage,
                    Priority = a.Priority,
                    TeamName = a.Team != null ? a.Team.Name : null,
                    CreatedAt = a.CreatedDate,
                    Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList()
                })
                .ToListAsync();

            methodResult.Result = articles;
            return methodResult;
        }
    }
}

