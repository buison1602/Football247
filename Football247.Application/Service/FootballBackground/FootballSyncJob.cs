using Football247.Application.SignalR;
using Football247.Domain.Models.EntityModels.DTOs.Match;
using Football247.Domain.Models.EntityModels.DTOs.Standing;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Football247.Application.Service.FootballBackground
{
    public class FootballSyncJob
    {
        private readonly FootballDataClient _client;
        private readonly IHubContext<FootballHub> _hub;
        private readonly ILogger<FootballSyncJob> _logger;

        // Map stage code → label tiếng Việt
        private static readonly Dictionary<string, string> StageLabels = new()
        {
            ["GROUP_STAGE"] = "Vòng bảng",
            ["LAST_32"] = "Vòng 1/16",
            ["LAST_16"] = "Vòng 1/8",
            ["QUARTER_FINALS"] = "Tứ kết",
            ["SEMI_FINALS"] = "Bán kết",
            ["THIRD_PLACE"] = "Tranh hạng ba",
            ["FINAL"] = "Chung kết",
        };

        // Thứ tự hiển thị stage
        private static readonly List<string> StageOrder = new()
        {
            "GROUP_STAGE", "LAST_32", "LAST_16",
            "QUARTER_FINALS", "SEMI_FINALS", "THIRD_PLACE", "FINAL"
        };

        public FootballSyncJob(
            FootballDataClient client,
            IHubContext<FootballHub> hub,
            ILogger<FootballSyncJob> logger)
        {
            _client = client;
            _hub = hub;
            _logger = logger;
        }

        // ── Sync matches: fetch → group → push hub ────────────────────────────
        public async Task SyncMatchesAsync(string competition, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("\n\n\n SyncMatchesAsync started+++++++++++++++++++++++++++++++++++++++++++++++++++++++++.\n\n\n\n");

                _logger.LogInformation("[{C}] Fetching matches...", competition);
                var json = await _client.GetAllMatchesAsync(competition, ct);
                var payload = ParseMatches(json, competition);

                await _hub.Clients
                    .Group($"{competition}-matches")
                    .SendAsync("MatchesUpdated", payload, ct);

                _logger.LogInformation(
                    "[{C}] Pushed {Stages} stages. HasLive={Live}",
                    competition, payload.Stages.Count, payload.HasLiveMatch);
            }
            catch (RateLimitException ex)
            {
                _logger.LogWarning("[{C}] Rate limit. Retry after {S}s", competition, ex.RetryAfter.TotalSeconds);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[{C}] Failed to sync matches.", competition);
            }
        }

        // ── Sync standings: fetch → flatten 48 đội → sort → push hub ─────────
        public async Task SyncStandingsAsync(string competition, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("\n\n\n SyncStandingsAsync started+++++++++++++++++++++++++++++++++++++++++++++++++++++++++.\n\n\n\n");

                _logger.LogInformation("[{C}] Fetching standings...", competition);
                var json = await _client.GetStandingsAsync(competition, ct);
                var payload = ParseStandings(json, competition);

                await _hub.Clients
                    .Group($"{competition}-standings")
                    .SendAsync("StandingsUpdated", payload, ct);

                _logger.LogInformation(
                    "[{C}] Pushed {Count} teams (standings).",
                    competition, payload.Standings.Count);
            }
            catch (RateLimitException ex)
            {
                _logger.LogWarning("[{C}] Rate limit. Retry after {S}s", competition, ex.RetryAfter.TotalSeconds);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[{C}] Failed to sync standings.", competition);
            }
        }

        // ── Parse matches → WcSchedulePayload ────────────────────────────────
        private static WcSchedulePayload ParseMatches(string json, string competition)
        {
            using var doc = JsonDocument.Parse(json);
            var payload = new WcSchedulePayload
            {
                CompetitionCode = competition,
                UpdatedAt = DateTime.UtcNow,
            };

            if (!doc.RootElement.TryGetProperty("matches", out var arr))
                return payload;

            // Parse tất cả matches
            var allMatches = new List<MatchFixtureDto>();
            foreach (var m in arr.EnumerateArray())
            {
                var status = m.GetProperty("status").GetString() ?? "";
                var stage = m.GetProperty("stage").GetString() ?? "";
                var group = m.TryGetProperty("group", out var g) ? g.GetString() : null;

                // Dòng này trong vòng foreach của ParseMatches
                var match = new MatchFixtureDto
                {
                    ExternalId = m.GetProperty("id").GetInt32(), // dòng này ok vì id trận luôn có
                    UtcDate = m.GetProperty("utcDate").GetDateTime(),
                    Status = m.GetProperty("status").GetString() ?? "",
                    Matchday = m.TryGetProperty("matchday", out var md)
                                && md.ValueKind != JsonValueKind.Null ? md.GetInt32() : 0,
                    Stage = m.TryGetProperty("stage", out var st)
                                && st.ValueKind != JsonValueKind.Null ? st.GetString() ?? "" : "",
                    Group = m.TryGetProperty("group", out var h)
                                && h.ValueKind != JsonValueKind.Null ? h.GetString() : null,
                    HomeTeam = ParseTeam(m.GetProperty("homeTeam")),  // ParseTeam đã fix null ở trên
                    AwayTeam = ParseTeam(m.GetProperty("awayTeam")),
                    HomeScore = TryGetScore(m, "home"),
                    AwayScore = TryGetScore(m, "away"),
                    Winner = m.TryGetProperty("score", out var sc)
                                && sc.TryGetProperty("winner", out var w)
                                && w.ValueKind != JsonValueKind.Null
                                ? w.GetString() : null,
                };

                allMatches.Add(match);

                if (match.IsLive)
                    payload.HasLiveMatch = true;
            }

            // Group theo Stage → Group, theo đúng thứ tự
            var byStage = allMatches
                .GroupBy(m => m.Stage)
                .OrderBy(g => StageOrder.IndexOf(g.Key) is var i && i >= 0 ? i : 99);

            foreach (var stageGroup in byStage)
            {
                var stageDto = new StageDto
                {
                    Stage = stageGroup.Key,
                    Label = StageLabels.GetValueOrDefault(stageGroup.Key, stageGroup.Key),
                };

                if (stageGroup.Key == "GROUP_STAGE")
                {
                    // Vòng bảng: chia theo group (GROUP_A ... GROUP_L)
                    var byGroup = stageGroup
                        .GroupBy(m => m.Group ?? "")
                        .OrderBy(g => g.Key); // A → L

                    foreach (var groupMatches in byGroup)
                    {
                        stageDto.Groups.Add(new GroupDto
                        {
                            GroupName = groupMatches.Key,
                            Matches = groupMatches
                                .OrderBy(m => m.UtcDate)
                                .ToList(),
                        });
                    }
                }
                else
                {
                    // Knockout: 1 group duy nhất, không tên group
                    stageDto.Groups.Add(new GroupDto
                    {
                        GroupName = "",
                        Matches = stageGroup
                            .OrderBy(m => m.UtcDate)
                            .ToList(),
                    });
                }

                payload.Stages.Add(stageDto);
            }

            return payload;
        }

        // ── Parse standings → flatten 48 đội, sort ───────────────────────────
        private static WcStandingsPayload ParseStandings(string json, string competition)
        {
            using var doc = JsonDocument.Parse(json);
            var payload = new WcStandingsPayload
            {
                CompetitionCode = competition,
                UpdatedAt = DateTime.UtcNow,
            };

            if (!doc.RootElement.TryGetProperty("standings", out var arr))
                return payload;

            var allTeams = new List<StandingDto>();

            foreach (var group in arr.EnumerateArray())
            {
                // Chỉ lấy type TOTAL (không lấy HOME/AWAY)
                var type = group.TryGetProperty("type", out var t) ? t.GetString() : "";
                var groupName = group.TryGetProperty("group", out var g) ? g.GetString() ?? "" : "";

                if (type != "TOTAL") continue;
                if (!group.TryGetProperty("table", out var table)) continue;

                foreach (var row in table.EnumerateArray())
                {
                    var team = row.GetProperty("team");
                    allTeams.Add(new StandingDto
                    {
                        Position = row.GetProperty("position").GetInt32(),
                        TeamExternalId = team.GetProperty("id").GetInt32(),
                        TeamName = team.GetProperty("name").GetString() ?? "",
                        TeamShortName = team.GetProperty("shortName").GetString() ?? "",
                        TeamCrest = team.GetProperty("crest").GetString() ?? "",
                        GroupName = groupName,
                        PlayedGames = row.GetProperty("playedGames").GetInt32(),
                        Won = row.GetProperty("won").GetInt32(),
                        Draw = row.GetProperty("draw").GetInt32(),
                        Lost = row.GetProperty("lost").GetInt32(),
                        Points = row.GetProperty("points").GetInt32(),
                        GoalsFor = row.GetProperty("goalsFor").GetInt32(),
                        GoalsAgainst = row.GetProperty("goalsAgainst").GetInt32(),
                        GoalDifference = row.GetProperty("goalDifference").GetInt32(),
                    });
                }
            }

            // Flatten + sort theo yêu cầu:
            // 1. Points DESC
            // 2. PlayedGames DESC
            // 3. GoalDifference DESC
            // 4. GoalsFor DESC
            // 5. GoalsAgainst ASC (ít bị thủng lưới hơn thì tốt hơn)
            payload.Standings = allTeams
                .OrderByDescending(t => t.Points)
                .ThenByDescending(t => t.PlayedGames)
                .ThenByDescending(t => t.GoalDifference)
                .ThenByDescending(t => t.GoalsFor)
                .ThenBy(t => t.GoalsAgainst)
                .Select((t, index) => { t.Position = index + 1; return t; })
                .ToList();

            return payload;
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static TeamInMatchDto ParseTeam(JsonElement t) => new()
        {
            ExternalId = t.TryGetProperty("id", out var id) && id.ValueKind != JsonValueKind.Null
                            ? id.GetInt32() : 0,
            Name = t.TryGetProperty("name", out var name) && name.ValueKind != JsonValueKind.Null
                            ? name.GetString() ?? "" : "",
            ShortName = t.TryGetProperty("shortName", out var sn) && sn.ValueKind != JsonValueKind.Null
                            ? sn.GetString() ?? "" : "",
            Tla = t.TryGetProperty("tla", out var tla) && tla.ValueKind != JsonValueKind.Null
                            ? tla.GetString() ?? "" : "",
            Crest = t.TryGetProperty("crest", out var crest) && crest.ValueKind != JsonValueKind.Null
                            ? crest.GetString() ?? "" : "",
        };

        private static int? TryGetScore(JsonElement m, string side)
        {
            if (!m.TryGetProperty("score", out var score)) return null;
            if (!score.TryGetProperty("fullTime", out var ft)) return null;
            if (!ft.TryGetProperty(side, out var val)) return null;
            return val.ValueKind == JsonValueKind.Null ? null : val.GetInt32();
        }
    }
}