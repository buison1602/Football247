using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Standing;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.StandingQuery
{
    public class GetAllStandingQuery : IRequest<MethodResult<List<StandingDto>>>
    {
    }

    public class GetAllStandingHandler : IRequestHandler<GetAllStandingQuery, MethodResult<List<StandingDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public GetAllStandingHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<List<StandingDto>>> Handle(GetAllStandingQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<StandingDto>>();

            var standings = await _unitOfWork.StandingRepository.ReadQueryable
                .ToListAsync(cancellationToken);

            methodResult.Result = _mapper.Map<List<StandingDto>>(standings);
            return methodResult;
        }
    }
}
