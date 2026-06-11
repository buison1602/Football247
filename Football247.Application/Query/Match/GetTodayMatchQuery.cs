using AutoMapper;
using Football247.Domain.Entities;
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
    public class GetTodayMatchQueryModel
    {
        public bool isResult { get; set; }
    }

    public class GetTodayMatchQuery : GetTodayMatchQueryModel, IRequest<MethodResult<List<MatchFixtureDto>>>
    {
    }

    public class GetTodayMatchHandler : IRequestHandler<GetTodayMatchQuery, MethodResult<List<MatchFixtureDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public GetTodayMatchHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<MethodResult<List<MatchFixtureDto>>> Handle(
    GetTodayMatchQuery request,
    CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);

            var methodResult = new MethodResult<List<MatchFixtureDto>>();
            var checkDate = DateTime.UtcNow.Date;
            var matches = new List<Domain.Entities.Match>();

            const int maxDaysRange = 30;
            int daysChecked = 0;

            // Nếu isResult = true  -> lấy kết quả các trận đã đá (lùi về quá khứ)
            // Nếu isResult = false -> lấy lịch thi đấu sắp tới (tiến về tương lai)
            var step = request.isResult ? -1 : 1;

            while (matches.Count < 5 && daysChecked < maxDaysRange)
            {
                IQueryable<Domain.Entities.Match> query =
                    _unitOfWork.MatchRepository.ReadQueryable
                        .Where(m => m.UtcDate.Date == checkDate);

                // Kết quả trận đấu
                if (request.isResult)
                {
                    query = query.Where(m => m.Status == EnumMatchStatus.Finished);
                }
                // Lịch thi đấu sắp tới
                else
                {
                    query = query.Where(m =>
                        m.Status == EnumMatchStatus.Timed ||
                        m.Status == EnumMatchStatus.Scheduled ||
                        m.Status == EnumMatchStatus.InPlay);
                }

                var dailyMatches = await query
                    .OrderBy(m => m.UtcDate)
                    .ToListAsync(cancellationToken);

                if (dailyMatches.Any())
                {
                    matches.AddRange(dailyMatches);
                }

                // Chuyển sang ngày tiếp theo hoặc ngày trước đó
                checkDate = checkDate.AddDays(step);
                daysChecked++;
            }

            // Chỉ lấy tối đa 5 trận
            matches = request.isResult
                ? matches
                    .OrderByDescending(m => m.UtcDate)
                    .Take(5)
                    .ToList()
                : matches
                    .OrderBy(m => m.UtcDate)
                    .Take(5)
                    .ToList();

            methodResult.Result = _mapper.Map<List<MatchFixtureDto>>(matches);

            return methodResult;
        }
    }
}
