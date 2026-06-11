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
    public class UpdateTeamCommand : UpdateTeamCommandModel, IRequest<MethodResult<TeamDto>>
    {
    }

    public class UpdateTeamHandler : IRequestHandler<UpdateTeamCommand, MethodResult<TeamDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateTeamHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<TeamDto>> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
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

            var team = await _unitOfWork.TeamRepository.GetByIdAsync(request.Id);

            if (team == null)
            {
                methodResult.AddError(StatusCodes.Status404NotFound, nameof(EnumSystemErrorCode.DataNotExist), nameof(request.Id), request.Id);
                return methodResult;
            }
            #endregion

            var teamDomain = _mapper.Map<Team>(request);
            await _unitOfWork.TeamRepository.UpdateAsync(request.Id, teamDomain);
            await _unitOfWork.SaveAsync();

            methodResult.Result = _mapper.Map<TeamDto>(teamDomain);
            return methodResult;
        }
    }
}
