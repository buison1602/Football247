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

namespace Football247.Application.Command.CommentCmd
{
    public class DeleteCommentCommand : IRequest<MethodResult<bool>>
    {
        public Guid CommentId { get; set; }
    }

    public class DeleteCommentCommandHandler : IRequestHandler<DeleteCommentCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteCommentCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CommentId), request.CommentId);
                return methodResult;
            }

            await _unitOfWork.CommentRepository.DeleteAsync(comment.Id);
            await _unitOfWork.SaveAsync();
            methodResult.Result = true;
            return methodResult;
        }
    }
}
