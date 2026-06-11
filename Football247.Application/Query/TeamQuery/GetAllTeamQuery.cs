using Football247.Domain.Models.EntityModels.DTOs.Team;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Football247.Application.Query.TeamQuery
{
    public class GetAllTeamQuery : IRequest<MethodResult<List<TeamDto>>>
    {
    }

    public class GetAllTeamQueryHandler : IRequestHandler<GetAllTeamQuery, MethodResult<List<TeamDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetAllTeamQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MethodResult<List<TeamDto>>> Handle(GetAllTeamQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<TeamDto>>();

            var teams = await _unitOfWork.TeamRepository.ReadQueryable
                .OrderByDescending(t => t.CreatedDate)
                .Select(t => new TeamDto
                {
                    Id = t.Id,
                    Name = t.Name,
                    Slug = t.Slug,
                    LogoUrl = t.LogoUrl
                })
                .ToListAsync(cancellationToken);

            methodResult.Result = teams;
            return methodResult;
        }
    }
}
