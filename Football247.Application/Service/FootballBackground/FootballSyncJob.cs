

using Football247.Application.SignalR;
using Football247.Domain.Entities;
using Football247.Repositories.IRepository;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Shared.Enum;
using System.Text.Json;

namespace Football247.Application.Service.FootballBackground
{
    public class FootballSyncJob
    {
        private const string Competition = "PL";

        private readonly FootballDataClient _client;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<FootballHub> _hub;
        private readonly ILogger<FootballSyncJob> _logger;

        public FootballSyncJob(
            FootballDataClient client,
            IUnitOfWork unitOfWork,
            IHubContext<FootballHub> hub,
            ILogger<FootballSyncJob> logger)
        {
            _client = client;
            _unitOfWork = unitOfWork;
            _hub = hub;
            _logger = logger;
        }

        // ── B1: Sync toàn bộ mùa giải (gọi 1 lần khi app start) ──────────────
        public async Task SyncFullSeasonAsync(CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("[PL] Syncing full season...");
                var json = await _client.GetFullSeasonAsync(Competition, ct);
                var matches = ParseMatches(json);

                foreach (var match in matches)
                {
                    var existing = await _unitOfWork.MatchRepository.ReadQueryable
                        .FirstOrDefaultAsync(m => m.ExternalId == match.ExternalId);

                    if (existing == null)
                        await _unitOfWork.MatchRepository.CreateAsync(match);
                    else
                        await _unitOfWork.MatchRepository.UpdateAsync(existing.Id, match);
                }

                await _unitOfWork.SaveAsync();
                _logger.LogInformation("[PL] Full season synced. Total: {Count} matches", matches.Count);

                // sync standings ngay sau khi xong matches
                await SyncStandingsAsync(ct);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[PL] Failed to sync full season.");
            }
        }

        // ── B3: Sync trận hôm nay (gọi theo interval khi có trận) ────────────
        // Trả về true nếu có match vừa FINISHED (để trigger sync standings)
        public async Task<SyncTodayResult> SyncTodayMatchesAsync(CancellationToken ct)
        {
            var result = new SyncTodayResult();
            try
            {
                var today = DateOnly.FromDateTime(DateTime.UtcNow);
                _logger.LogInformation("[PL] Syncing today's matches ({Date})...", today);

                var json = await _client.GetMatchesByDateAsync(Competition, today, ct);
                var apiMatches = ParseMatches(json);

                result.HasLiveMatch = apiMatches.Any(m =>
                    m.Status is EnumMatchStatus.InPlay or EnumMatchStatus.Paused);

                var changedMatches = new List<Match>();

                foreach (var apiMatch in apiMatches)
                {
                    var existing = await _unitOfWork.MatchRepository.ReadQueryable
                       .FirstOrDefaultAsync(m => m.ExternalId == apiMatch.ExternalId);

                    if (existing == null)
                    {
                        await _unitOfWork.MatchRepository.CreateAsync(apiMatch);
                        changedMatches.Add(apiMatch);
                        continue;
                    }

                    // Chỉ update nếu có thay đổi thật sự
                    bool statusChanged = existing.Status != apiMatch.Status;
                    bool scoreChanged = existing.HomeScore != apiMatch.HomeScore
                                     || existing.AwayScore != apiMatch.AwayScore;

                    if (statusChanged || scoreChanged)
                    {
                        // Phát hiện match vừa FINISHED → cần sync standings
                        if (apiMatch.Status == EnumMatchStatus.Finished
                            && existing.Status != EnumMatchStatus.Finished)
                        {
                            result.AnyJustFinished = true;
                            _logger.LogInformation("[PL] Match {Id} just finished. Will sync standings.", apiMatch.ExternalId);
                        }

                        await _unitOfWork.MatchRepository.UpdateAsync(existing.Id, apiMatch);
                        changedMatches.Add(apiMatch);
                    }
                }

                if (changedMatches.Count > 0)
                {
                    await _unitOfWork.SaveAsync();

                    // Bắn Hub cho user đang online
                    await _hub.Clients
                        .Group("PL-matches")
                        .SendAsync("MatchesUpdated", changedMatches, ct);

                    _logger.LogInformation("[PL] {Count} matches updated and pushed to Hub.", changedMatches.Count);
                }

                result.AllFinished = apiMatches.All(m =>
                    m.Status is EnumMatchStatus.Finished
                             or EnumMatchStatus.Postponed
                             or EnumMatchStatus.Cancelled);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[PL] Failed to sync today's matches.");
            }

            return result;
        }

        // ── B4: Sync standings (chỉ gọi khi có match vừa FINISHED) ───────────
        public async Task SyncStandingsAsync(CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("[PL] Syncing standings after match finished...");
                var json = await _client.GetStandingsAsync(Competition, ct);
                var standings = ParseStandings(json);

                foreach (var standing in standings)
                {
                    var existing = await _unitOfWork.StandingRepository.ReadQueryable
                        .FirstOrDefaultAsync(s => s.CompetitionCode == Competition
                                   && s.Season == standing.Season
                                   && s.TeamExternalId == standing.TeamExternalId);

                    if (existing == null)
                        await _unitOfWork.StandingRepository.CreateAsync(standing);
                    else
                        await _unitOfWork.StandingRepository.UpdateAsync(existing.Id, standing);
                }

                await _unitOfWork.SaveAsync();

                // Bắn Hub standings
                await _hub.Clients
                    .Group("PL-standings")
                    .SendAsync("StandingsUpdated", standings, ct);

                _logger.LogInformation("[PL] Standings synced and pushed to Hub.");
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[PL] Failed to sync standings.");
            }
        }

        // ── Parse helpers ──────────────────────────────────────────────────────
        private static List<Match> ParseMatches(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var matches = new List<Match>();

            if (!doc.RootElement.TryGetProperty("matches", out var arr)) return matches;

            foreach (var m in arr.EnumerateArray())
            {
                matches.Add(new Match
                {
                    ExternalId = m.GetProperty("id").GetInt32(),
                    UtcDate = m.GetProperty("utcDate").GetDateTime(),
                    Status = ParseStatus(m.GetProperty("status").GetString()!),
                    Matchday = m.TryGetProperty("matchday", out var md) ? md.GetInt32() : 0,
                    HomeTeamExternalId = m.GetProperty("homeTeam").GetProperty("id").GetInt32(),
                    HomeTeamName = m.GetProperty("homeTeam").GetProperty("name").GetString()!,
                    HomeTeamShortName = m.GetProperty("homeTeam").GetProperty("shortName").GetString()!,
                    HomeTeamCrest = m.GetProperty("homeTeam").GetProperty("crest").GetString()!,
                    AwayTeamExternalId = m.GetProperty("awayTeam").GetProperty("id").GetInt32(),
                    AwayTeamName = m.GetProperty("awayTeam").GetProperty("name").GetString()!,
                    AwayTeamShortName = m.GetProperty("awayTeam").GetProperty("shortName").GetString()!,
                    AwayTeamCrest = m.GetProperty("awayTeam").GetProperty("crest").GetString()!,
                    HomeScore = TryGetScore(m, "home"),
                    AwayScore = TryGetScore(m, "away"),
                    CompetitionCode = "PL",
                    CompetitionName = "Premier League",
                    Season = m.GetProperty("season").GetProperty("startDate").GetDateTime().Year,
                });
            }

            return matches;
        }

        private static List<Standing> ParseStandings(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var standings = new List<Standing>();

            var table = doc.RootElement
                .GetProperty("standings")
                .EnumerateArray()
                .First(s => s.GetProperty("type").GetString() == "TOTAL")
                .GetProperty("table");

            var season = doc.RootElement
                .GetProperty("season")
                .GetProperty("startDate")
                .GetDateTime().Year;

            foreach (var row in table.EnumerateArray())
            {
                standings.Add(new Standing
                {
                    CompetitionCode = "PL",
                    Season = season,
                    Position = row.GetProperty("position").GetInt32(),
                    TeamExternalId = row.GetProperty("team").GetProperty("id").GetInt32(),
                    TeamName = row.GetProperty("team").GetProperty("name").GetString()!,
                    TeamShortName = row.GetProperty("team").GetProperty("shortName").GetString()!,
                    TeamCrest = row.GetProperty("team").GetProperty("crest").GetString()!,
                    PlayedGames = row.GetProperty("playedGames").GetInt32(),
                    Won = row.GetProperty("won").GetInt32(),
                    Draw = row.GetProperty("draw").GetInt32(),
                    Lost = row.GetProperty("lost").GetInt32(),
                    GoalDifference = row.GetProperty("goalDifference").GetInt32(),
                    Points = row.GetProperty("points").GetInt32(),
                    LastSyncedAt = DateTime.UtcNow,
                });
            }

            return standings;
        }

        private static int? TryGetScore(JsonElement match, string side)
        {
            if (!match.TryGetProperty("score", out var score)) return null;
            if (!score.TryGetProperty("fullTime", out var ft)) return null;
            if (!ft.TryGetProperty(side, out var val)) return null;
            return val.ValueKind == JsonValueKind.Null ? null : val.GetInt32();
        }

        private static EnumMatchStatus ParseStatus(string s) => s switch
        {
            "TIMED" => EnumMatchStatus.Timed,
            "SCHEDULED" => EnumMatchStatus.Scheduled,
            "LIVE" => EnumMatchStatus.Live,
            "IN_PLAY" => EnumMatchStatus.InPlay,
            "PAUSED" => EnumMatchStatus.Paused,
            "FINISHED" => EnumMatchStatus.Finished,
            "POSTPONED" => EnumMatchStatus.Postponed,
            "SUSPENDED" => EnumMatchStatus.Suspended,
            "CANCELLED" => EnumMatchStatus.Cancelled,
            _ => throw new ArgumentOutOfRangeException($"Unknown status: {s}")
        };
    }

    // Result object để truyền thông tin giữa Job và BackgroundService
    public class SyncTodayResult
    {
        public bool HasLiveMatch { get; set; }
        public bool AnyJustFinished { get; set; }
        public bool AllFinished { get; set; }
    }
}
