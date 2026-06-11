using Football247.Domain.Models.CommandModels.AuthCommand;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.AuthCmd
{
    public class LogoutCommand : LogoutCommandModel, IRequest<MethodResult<bool>>
    {
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, MethodResult<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public LogoutCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<bool>> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<bool>();

            if (string.IsNullOrEmpty(request.RefreshToken))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.RefreshToken));
                return methodResult;
            }
            
            var result = await _unitOfWork.TokenRepository.LogoutAsync(request.RefreshToken);
            if (result != true)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(request.RefreshToken));
                return methodResult;
            }

            methodResult.Result = true;
            methodResult.StatusCode = StatusCodes.Status200OK;
            return methodResult;
        }
    }
}
