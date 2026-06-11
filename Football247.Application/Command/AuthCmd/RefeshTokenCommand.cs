using Football247.Domain.Models.CommandModels.AuthCommand;
using Football247.Domain.Models.EntityModels.DTOs.Auth;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.AuthCmd
{
    public class RefeshTokenCommand : RefeshTokenCommandModel, IRequest<MethodResult<AuthResultDto>>
    {
    }

    public class RefeshTokenCommandHandler : IRequestHandler<RefeshTokenCommand, MethodResult<AuthResultDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public RefeshTokenCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<AuthResultDto>> Handle(RefeshTokenCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<AuthResultDto>();

            if (request.RefreshToken == null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.RefreshToken));
                return methodResult;
            }

            var result = await _unitOfWork.TokenRepository.RefreshTokensAsync(request.RefreshToken);
            methodResult.Result = new AuthResultDto
            {
                UserId = result.UserId.ToString(),
                FullName = result.FullName,
                JwtToken = result.JwtToken,
                RefreshToken = result.RefreshToken
            };

            return methodResult;
        }
    }
}
