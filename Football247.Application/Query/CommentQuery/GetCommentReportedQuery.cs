using Football247.Domain.Models.EntityModels.DTOs.Comment;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;

namespace Football247.Application.Query.CommentQuery
{
    public class GetCommentReportedQuery : IRequest<MethodResult<List<CommentDto>>>
    {
    }

    public class GetCommentReportedQueryHandler : IRequestHandler<GetCommentReportedQuery, MethodResult<List<CommentDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetCommentReportedQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<CommentDto>>> Handle(GetCommentReportedQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<CommentDto>>();

            var comments = await _unitOfWork.CommentRepository.ReadQueryable
                .Where(c => c.IsReported == true)
                .Select(c => new CommentDto
                {
                    Id = c.Id,
                    Content = c.Content,
                    CreatedAt = c.CreatedDate,
                    CreatorId = c.CreatedUserId,
                    CreatorName = c.CreatedFullName,
                    ArticleId = c.ArticleId
                }).ToListAsync(cancellationToken);

            methodResult.Result = comments;
            return methodResult;
        }
    }
}