using AutoMapper;
using Football247.Domain.Entities;
using Football247.Domain.Models.CommandModels.TeamCmdModel;
using Football247.Domain.Models.EntityModels.DTOs.Team;
using Football247.Models.Entities;
using Football247.Repositories.IRepository;
using Football247.Shared.Enum.ErrorCode;
using MediatR;
using Microsoft.AspNetCore.Http;
using Shared.Response;

namespace Football247.Application.Command.TeamCmd
{
    public class CreateTeamCommand : CreateTeamCommandModel, IRequest<MethodResult<TeamDto>>
    {
    }

    public class CreateTeamHandler : IRequestHandler<CreateTeamCommand, MethodResult<TeamDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateTeamHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<TeamDto>> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<TeamDto>();

            #region Validation
            if (string.IsNullOrEmpty(request.Name))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Name));
                return methodResult;
            }

            if (string.IsNullOrEmpty(request.Slug))
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.Required), nameof(request.Slug));
                return methodResult;
            }

            var team = await _unitOfWork.TeamRepository.GetBySlugAsync(request.Slug);

            if (team != null)
            {
                methodResult.AddError(StatusCodes.Status400BadRequest, nameof(EnumSystemErrorCode.DataAlreadyExist), nameof(request.Slug), request.Slug);
                return methodResult;
            }
            #endregion

            var teamDomain = _mapper.Map<Team>(request);
            if (teamDomain == null) 
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(request));
                return methodResult;
            }

            if (string.IsNullOrWhiteSpace(teamDomain.LogoUrl))
            {
                teamDomain.LogoUrl = "https://link-anh-mac-dinh.png";
            }

            teamDomain = await _unitOfWork.TeamRepository.CreateAsync(teamDomain);
            if (teamDomain == null)
            {
                methodResult.AddError(StatusCodes.Status500InternalServerError, nameof(EnumSystemErrorCode.ServerError), nameof(request));
                return methodResult;
            }

            methodResult.StatusCode = StatusCodes.Status200OK;
            methodResult.Result = _mapper.Map<TeamDto>(teamDomain);
            return methodResult;
        }
    }
}
