using AutoMapper;
using Football247.Application.SignalR;
using Football247.Domain.Models.CommandModels.CommentCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Comment;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Shared.Response;

namespace Football247.Application.Command.CommentCmd
{
    public class CreateCommentCommand : CreateCommentCommandModel, IRequest<MethodResult<CommentDto>>
    {
    }

    public class CreateCommentCommandHandler : IRequestHandler<CreateCommentCommand, MethodResult<CommentDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<ArticleCommentHub> _hubContext;
        public CreateCommentCommandHandler(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IHubContext<ArticleCommentHub> hubContext)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
        }
        public async Task<MethodResult<CommentDto>> Handle(CreateCommentCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<CommentDto>();
            if (string.IsNullOrWhiteSpace(request.Content))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest,
                    nameof(EnumSystemErrorCode.Required), nameof(request.Content));
                return methodResult;
            }
            if (request.ArticleId == Guid.Empty)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest,
                    nameof(EnumSystemErrorCode.Required), nameof(request.ArticleId));
                return methodResult;
            }
            
            var comment = _mapper.Map<Comment>(request);
            comment = await _unitOfWork.CommentRepository.CreateAsync(comment);
            
            if (comment == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError,
                    nameof(EnumSystemErrorCode.ServerError), nameof(comment));
                return methodResult;
            }

            await _unitOfWork.SaveAsync();

            var commentDto = _mapper.Map<CommentDto>(comment);
            
            await _hubContext.Clients
                .Group($"article_{request.ArticleId.ToString().ToLower()}")
                .SendAsync("ReceiveComment", commentDto, cancellationToken);
            
            methodResult.Result = commentDto;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
