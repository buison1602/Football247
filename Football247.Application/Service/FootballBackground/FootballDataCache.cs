using Football247.Domain.Models.EntityModels.DTOs.Match;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Application.Service.FootballBackground
{
    public class FootballDataCache
    {
        // Lưu trữ kết quả matches theo CompetitionCode
        public ConcurrentDictionary<string, object> Matches { get; } = new();

        // Lưu trữ kết quả standings theo CompetitionCode
        public ConcurrentDictionary<string, object> Standings { get; } = new();

        // Lưu trữ chi tiết trận đấu theo MatchId
        //public ConcurrentDictionary<int, object> MatchDetails { get; } = new();

        // 537400 → MatchDetailPayload (chỉ có trận đang live)
        public ConcurrentDictionary<int, MatchDetailPayload> MatchDetails { get; } = new();

        // Danh sách matchId đang live (để background biết cần poll detail)
        public ConcurrentDictionary<int, string> LiveMatchIds { get; } = new();
        // key = matchId, value = competitionCode
    }
}
