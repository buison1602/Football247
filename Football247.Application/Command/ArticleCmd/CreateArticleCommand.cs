using AutoMapper;
using Football247.Domain.Models.CommandModels.ArticleCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Enum;
using Shared.Response;

namespace Football247.Application.Command.ArticleCmd
{
    public class CreateArticleCommand : CreateArticleCommandModel, IRequest<MethodResult<ArticleDto>>
    {
    }

    public class CreateArticleHandler : IRequestHandler<CreateArticleCommand, MethodResult<ArticleDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateArticleHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<ArticleDto>> Handle(CreateArticleCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<ArticleDto>();

            #region Validate dữ liệu từ request
            if (string.IsNullOrEmpty(request.Title))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Title));
                return methodResult;
            }

            if (string.IsNullOrEmpty(request.Slug))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Slug));
                return methodResult;
            }

            if (string.IsNullOrEmpty(request.Description))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Description));
                return methodResult;
            }

            if (string.IsNullOrEmpty(request.Content))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Content));
                return methodResult;
            }

            if (request.CategoryId == Guid.Empty)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.CategoryId));
                return methodResult;
            }

            var existingArticle = await _unitOfWork.ArticleRepository.GetBySlugAsync(request.Slug);
            if (existingArticle != null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Slug));
                return methodResult;
            }
            #endregion

            var articleDomain = _mapper.Map<Article>(request);
            articleDomain.Status = EnumStatusArticle.Pending;   

            articleDomain = await _unitOfWork.ArticleRepository.CreateAsync(articleDomain);

            if (articleDomain == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(articleDomain));
                return methodResult;
            }

            await _unitOfWork.SaveAsync();

            methodResult.Result = _mapper.Map<ArticleDto>(articleDomain);
            methodResult.StatusCode = StatusCodes.Status200OK;

            return methodResult;
        }
    }
}