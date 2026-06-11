using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Comment;
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
    public class ReportCommentCommand : IRequest<MethodResult<bool>>
    {
        public Guid CommentId { get; set; }
    }

    public class ReportCommentCommandHandler : IRequestHandler<ReportCommentCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ReportCommentCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<bool>> Handle(ReportCommentCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            var comment = await _unitOfWork.CommentRepository.GetByIdAsync(request.CommentId);
            if (comment == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.CommentId), request.CommentId);
                return methodResult;
            }

            comment.IsReported = true;
            await _unitOfWork.SaveAsync();

            methodResult.IsSuccess = true;
            return methodResult;
        }
    }
}
