using AutoMapper;
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
    public class GetAllArticlesQueryModel : BaseQueryModel
    {
    }

    public class GetAllArticlesQuery : GetAllArticlesQueryModel, IRequest<MethodResult<PagingItemsModel<ArticlesDto>>>
    {
    }

    public class GetAllArticlesQueryHandler : IRequestHandler<GetAllArticlesQuery, MethodResult<PagingItemsModel<ArticlesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;   

        public GetAllArticlesQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<PagingItemsModel<ArticlesDto>>> Handle(GetAllArticlesQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<ArticlesDto>>();

            var query = _unitOfWork.ArticleRepository.ReadQueryable
                .Where(a => a.Status == EnumStatusArticle.Approved)
                .OrderByDescending(a => a.CreatedDate);

            var totalItems = await query.CountAsync(cancellationToken);

            var articleDtos = await query
                .OrderByDescending(a => a.CreatedDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(a => new ArticlesDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    Slug = a.Slug,
                    Description = a.Description,
                    BgrImage = a.BgrImage,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedDate, 
                    Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList() 
                })
                .ToListAsync(cancellationToken);

            methodResult.Result = new PagingItemsModel<ArticlesDto>(articleDtos, request, totalItems);
            return methodResult;
        }
    }
}
