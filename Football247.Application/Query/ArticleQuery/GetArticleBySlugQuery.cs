using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.ArticleQuery
{
    public class GetArticleBySlugQueryModel
    {
        public string Slug { get; set; } = string.Empty;
    }

    public class GetArticleBySlugQuery : GetArticleBySlugQueryModel, IRequest<MethodResult<ArticleDto>>
    {
    }

    public class GetArticleBySlugQueryHandler : IRequestHandler<GetArticleBySlugQuery, MethodResult<ArticleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetArticleBySlugQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<ArticleDto>> Handle(GetArticleBySlugQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ArticleDto>();
            ArticleDto article = await _unitOfWork.ArticleRepository.GetBySlugAsync(request.Slug);
            if (article == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Slug), request.Slug);
                return methodResult;
            }
            methodResult.Result = _mapper.Map<ArticleDto>(article);
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
