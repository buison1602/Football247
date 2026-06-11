using AutoMapper;
using Football247.Domain.Models.EntityModels.DTOs.Match;
using Football247.Repositories.IRepository;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Shared.Enum;
using Shared.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Query.Match
{
    public class GetAllMatchQueryModel
    {
        public bool isResult { get; set; }
    }
    public class GetAllMatchQuery : GetAllMatchQueryModel, IRequest<MethodResult<List<MatchFixtureDto>>>
    {
    }

    public class GetAllMatchHandler : IRequestHandler<GetAllMatchQuery, MethodResult<List<MatchFixtureDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetAllMatchHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<List<MatchFixtureDto>>> Handle(GetAllMatchQuery request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            var methodResult = new MethodResult<List<MatchFixtureDto>>();

            var today = DateTime.UtcNow.Date;

            // Query gốc
            var query = _unitOfWork.MatchRepository.ReadQueryable;

            if (request.isResult)
            {
                // Lấy tất cả các trận đã diễn ra từ hôm nay trở về trước
                query = query.Where(m =>
                    m.UtcDate.Date <= today &&
                    m.Status == EnumMatchStatus.Finished);
            }
            else
            {
                // Lấy toàn bộ trận đấu trong database
                // Không cần thêm điều kiện gì
            }

            var matches = await query
                .OrderByDescending(m => m.UtcDate)
                .ToListAsync(cancellationToken);

            methodResult.Result = _mapper.Map<List<MatchFixtureDto>>(matches);

            return methodResult;
        }
    }
}
