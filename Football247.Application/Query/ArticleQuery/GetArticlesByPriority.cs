using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Article;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace Football247.Application.Query.ArticleQuery
{
    public class GetArticlesByPriority : IRequest<MethodResult<List<ArticlesDto>>>
    {
    }

    public class GetArticlesByPriorityHandler : IRequestHandler<GetArticlesByPriority, MethodResult<List<ArticlesDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetArticlesByPriorityHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<List<ArticlesDto>>> Handle(GetArticlesByPriority request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<ArticlesDto>>();
            var articles = await _unitOfWork.ArticleRepository.ReadQueryable
                .Where(a => a.Priority == 2)
                .OrderByDescending(a => a.CreatedDate)
                .Take(10)
                .ToListAsync(cancellationToken);

            methodResult.Result = _mapper.Map<List<ArticlesDto>>(articles);
            return methodResult;
        }
    }
}
