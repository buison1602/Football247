using Football247.Application.Service.FootballBackground;
using Football247.Domain.Models.EntityModels.DTOs.Match;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Football247.Application.SignalR
{
    /// <summary>
    /// Hub xử lý realtime cho football data:
    /// matches, standings.
    /// AllowAnonymous vì guest cũng xem được lịch thi đấu.
    /// </summary>
    [AllowAnonymous]
    public class FootballHub : BaseHub
    {
        private readonly FootballDataCache _cache;

        public FootballHub(FootballDataCache cache)
        {
            _cache = cache;
        }

        public object GetInitialMatches(string competition)
        {
            Console.WriteLine("\n\n\n GetInitialMatches started++++++++++++++++++-----------0000000000000----------+++++++++++++++++++++.\n\n\n\n");

            if (_cache.Matches.TryGetValue(competition.ToUpper(), out var data))
            {
                return data;
            }
            return null; // Hoặc một object thông báo trống dữ liệu
        }

        // Hàm cho FE Invoke lấy Bảng xếp hạng lập tức
        public object GetInitialStandings(string competition)
        {
            if (_cache.Standings.TryGetValue(competition.ToUpper(), out var data))
            {
                return data;
            }
            return null;
        }

        // Hàm cho FE Invoke lấy Chi tiết trận đấu lập tức
        //public object GetInitialMatchDetail(int matchId)
        //{
        //    if (_cache.MatchDetails.TryGetValue(matchId, out var data))
        //    {
        //        return data;
        //    }
        //    return null;
        //}
        // 🌟 BÍ QUYẾT Ở ĐÂY: Cho FE Invoke lấy Chi tiết trận đấu
        // Tìm trực tiếp trong Cache của danh sách Matches, KHÔNG gọi API ngoài
        //public object GetInitialMatchDetail(int matchId)
        //{
        //    // Duyệt qua cache của các giải đấu đang có (PL, WC...)
        //    foreach (var cacheEntry in _cache.Matches)
        //    {
        //        if (cacheEntry.Value is WcSchedulePayload payload)
        //        {
        //            // Lục tìm trong các vòng đấu (Stage) và nhóm (Group)
        //            var match = payload.Stages
        //                .SelectMany(s => s.Groups)
        //                .SelectMany(g => g.Matches)
        //                .FirstOrDefault(m => m.ExternalId == matchId);

        //            if (match != null)
        //            {
        //                return match; // Tìm thấy phát là trả về luôn
        //            }
        //        }
        //    }

        //    return null; // Không tìm thấy
        //}
        // Hàm cho FE Invoke lấy Chi tiết trận đấu lập tức
        // Ưu tiên 1: Cache MatchDetails (đầy đủ referees, venue, halfTime...)
        //            → có khi trận đã/đang live (background đã gọi SyncMatchDetailAsync)
        // Ưu tiên 2 (fallback): Cache Matches (MatchFixtureDto - thiếu referees/venue)
        //            → dùng khi trận chưa từng live, FE vẫn có data cơ bản để hiển thị
        public object GetInitialMatchDetail(int matchId)
        {
            // Ưu tiên 1: đã có data chi tiết đầy đủ trong cache
            if (_cache.MatchDetails.TryGetValue(matchId, out var detail))
            {
                return detail; // MatchDetailPayload — có referees, venue, halfTime
            }

            // Ưu tiên 2: fallback — trận chưa từng được sync detail
            // Trả về dạng bọc tương tự MatchDetailPayload để FE không phải check 2 shape khác nhau
            foreach (var cacheEntry in _cache.Matches)
            {
                if (cacheEntry.Value is WcSchedulePayload payload)
                {
                    var match = payload.Stages
                        .SelectMany(s => s.Groups)
                        .SelectMany(g => g.Matches)
                        .FirstOrDefault(m => m.ExternalId == matchId);

                    if (match != null)
                    {
                        // Map sang MatchDetailDto rỗng phần referees/venue
                        // để FE luôn nhận đúng 1 shape duy nhất
                        return new MatchDetailPayload
                        {
                            UpdatedAt = DateTime.UtcNow,
                            Match = new MatchDetailDto
                            {
                                ExternalId = match.ExternalId,
                                UtcDate = match.UtcDate,
                                Status = match.Status,
                                Matchday = match.Matchday,
                                Stage = match.Stage,
                                Group = match.Group,
                                Venue = null,         // chưa có data
                                HomeTeam = match.HomeTeam,
                                AwayTeam = match.AwayTeam,
                                Score = new MatchScoreDto
                                {
                                    Winner = match.Winner,
                                    Duration = "REGULAR",
                                    FullTime = new ScoreLineDto { Home = match.HomeScore, Away = match.AwayScore },
                                    HalfTime = new ScoreLineDto { Home = null, Away = null },
                                },
                                Referees = new List<RefereeDto>(), // chưa có data
                            },
                        };
                    }
                }
            }

            return null; // Không tìm thấy ở đâu cả
        }

        protected override async Task OnHubConnectedAsync()
        {
            var http = Context.GetHttpContext();

            // Client kết nối kèm query param: ?competition=PL
            var competition = http?.Request.Query["competition"].ToString().ToUpper();

            if (!string.IsNullOrEmpty(competition))
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, $"{competition}-matches");
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, $"{competition}-standings");
            }

            var matchId = http?.Request.Query["matchId"].ToString();

            if (!string.IsNullOrEmpty(matchId))
            {
                await Groups.AddToGroupAsync(
                    Context.ConnectionId, $"match-{matchId}");
            }
        }

        protected override async Task OnHubDisconnectedAsync(Exception? exception)
        {
            var http = Context.GetHttpContext();

            var competition = http?.Request.Query["competition"].ToString().ToUpper();

            if (!string.IsNullOrEmpty(competition))
            {
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId, $"{competition}-matches");
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId, $"{competition}-standings");
            }

            var matchId = http?.Request.Query["matchId"].ToString();

            if (!string.IsNullOrEmpty(matchId))
            {
                await Groups.RemoveFromGroupAsync(
                    Context.ConnectionId, $"match-{matchId}");
            }
        }
    }
}
