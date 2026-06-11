using Football247.Domain.Models.EntityModels.DTOs.Comment;
using Football247.Repositories.IRepository;
using MediatR;
using Shared.Response;

namespace Football247.Application.Query.CommentQuery
{
    public class GetCommentsByArticleQueryModel
    {
        public Guid ArticleId { get; set; }
    }

    public class GetCommentsByArticleQuery : GetCommentsByArticleQueryModel, IRequest<MethodResult<List<CommentDto>>>
    {
    }

    public class GetCommentsByArticleHandler : IRequestHandler<GetCommentsByArticleQuery, MethodResult<List<CommentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCommentsByArticleHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<CommentDto>>> Handle(GetCommentsByArticleQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<CommentDto>>();

            var comments = await _unitOfWork.CommentRepository.GetCommentsByArticleIdAsync(request.ArticleId);

            methodResult.Result = comments;
            methodResult.StatusCode = 200;

            return methodResult;
        }
    }
}
