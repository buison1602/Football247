using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Query.ArticleQuery
{
    public class GetArticleByIdQueryModel
    {
        public Guid Id { get; set; }
    }

    public class GetArticleByIdQuery : GetArticleByIdQueryModel, IRequest<MethodResult<ArticleDto>>
    {
    }

    public class GetArticleByIdQueryHandler : IRequestHandler<GetArticleByIdQuery, MethodResult<ArticleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetArticleByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<ArticleDto>> Handle(GetArticleByIdQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ArticleDto>();

            Article article = await _unitOfWork.ArticleRepository.GetByIdAsync(request.Id);
            if (article == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }

            methodResult.Result = _mapper.Map<ArticleDto>(article);
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
