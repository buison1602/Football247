using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.ArticleQuery
{
    public class GetLatestArticleQuery : IRequest<MethodResult<List<ArticlesDto>>>
    {
    }

    public class GetLatestArticleQueryHandler : IRequestHandler<GetLatestArticleQuery, MethodResult<List<ArticlesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        public GetLatestArticleQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<ArticlesDto>>> Handle(GetLatestArticleQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<ArticlesDto>>();

            var query = _unitOfWork.ArticleRepository.ReadQueryable
                .Where(a => a.Status == EnumStatusArticle.Approved)
                .OrderByDescending(a => a.CreatedDate);

            var articles = await query
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
                .Take(5)
                .ToListAsync(cancellationToken);

            methodResult.Result = articles;
            return methodResult;
        }
    }
}
