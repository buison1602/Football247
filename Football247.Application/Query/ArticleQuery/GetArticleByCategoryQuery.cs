using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Shared.Common.Models.Paging;
using Shared.Response;


namespace Football247.Application.Query.ArticleQuery
{
    public class GetArticleByCategoryQueryModel : BaseQueryModel
    {
        public string CategorySlug { get; set; } = string.Empty;
    }

    public class GetArticleByCategoryQuery : GetArticleByCategoryQueryModel, IRequest<MethodResult<PagingItemsModel<ArticlesDto>>>
    {
    }

    public class GetArticleByCategoryQueryHandler : IRequestHandler<GetArticleByCategoryQuery, MethodResult<PagingItemsModel<ArticlesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetArticleByCategoryQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<PagingItemsModel<ArticlesDto>>> Handle(GetArticleByCategoryQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<PagingItemsModel<ArticlesDto>>();
            var category = await _unitOfWork.CategoryRepository.GetBySlugAsync(request.CategorySlug);

            if (category is null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CategorySlug), request.CategorySlug);
                return methodResult;
            }

            var query = _unitOfWork.ArticleRepository.ReadQueryable
                .Where(a => a.CategoryId == category.Id && a.IsApproved == false)
                .OrderByDescending(a => a.CreatedDate);

            var totalItems = await query.CountAsync(cancellationToken);

            var articles = await query
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
                    TeamName = a.Team.Name,
                    Priority = a.Priority,
                    CreatedAt = a.CreatedDate,
                    Tags = a.ArticleTags.Select(at => at.Tag.Name).ToList()
                })
                .ToListAsync(cancellationToken);

            var articleDtos = _mapper.Map<List<ArticlesDto>>(articles);
            methodResult.Result = new PagingItemsModel<ArticlesDto>(articleDtos, request, totalItems);
            return methodResult;
        }
    }
}