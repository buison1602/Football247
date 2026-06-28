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
        private readonly FootballDataCache _cache;

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

        private static readonly List<string> StageOrder = new()
        {
            "GROUP_STAGE", "LAST_32", "LAST_16",
            "QUARTER_FINALS", "SEMI_FINALS", "THIRD_PLACE", "FINAL"
        };

        public FootballSyncJob(
            FootballDataClient client,
            IHubContext<FootballHub> hub,
            ILogger<FootballSyncJob> logger,
            FootballDataCache cache)
        {
            _client = client;
            _hub = hub;
            _logger = logger;
            _cache = cache;
        }

        // ── Sync danh sách trận ───────────────────────────────────────────────
        public async Task SyncMatchesAsync(string competition, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("[{C}] Fetching matches...", competition);
                var json = await _client.GetAllMatchesAsync(competition, ct);
                var payload = ParseMatches(json, competition);

                _cache.Matches[competition] = payload;

                // Cập nhật danh sách live match ids
                _cache.LiveMatchIds.Clear();
                foreach (var stage in payload.Stages)
                    foreach (var group in stage.Groups)
                        foreach (var match in group.Matches.Where(m => m.IsLive))
                        {
                            _cache.LiveMatchIds[match.ExternalId] = competition;
                        }

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

        // ── Sync bảng xếp hạng ───────────────────────────────────────────────
        public async Task SyncStandingsAsync(string competition, CancellationToken ct)
        {
            try
            {
                _logger.LogInformation("[{C}] Fetching standings...", competition);
                var json = await _client.GetStandingsAsync(competition, ct);
                var payload = ParseStandings(json, competition);

                _cache.Standings[competition] = payload;

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

        // ── Sync chi tiết 1 trận theo matchId ────────────────────────────────
        // Gửi lên group "match-{matchId}" → event "MatchDetailUpdated"
        public async Task SyncLiveMatchDetailsAsync(CancellationToken ct)
        {
            _logger.LogInformation("\n\n\n SyncLiveMatchDetailsAsync started+++++++++++++++++++++++++++++++++++++++++++++++++++++++++.\n\n\n\n");

            var liveIds = _cache.LiveMatchIds.Keys.ToList();

            if (liveIds.Count == 0)
            {
                _logger.LogDebug("[MatchDetail] Không có trận live → skip.");
                return;
            }

            _logger.LogInformation(
                "[MatchDetail] Syncing {Count} live matches: {Ids}",
                liveIds.Count, string.Join(", ", liveIds));

            foreach (var matchId in liveIds)
            {
                if (ct.IsCancellationRequested) break;
                await SyncMatchDetailAsync(matchId, ct);

                if (matchId != liveIds.Last())
                    await Task.Delay(TimeSpan.FromSeconds(1), ct);
            }
        }

        // ── 4. Sync detail 1 trận cụ thể ─────────────────────────────────────
        public async Task SyncMatchDetailAsync(int matchId, CancellationToken ct)
        {
            try
            {
                var json = await _client.GetMatchDetailAsync(matchId, ct);
                var payload = ParseMatchDetail(json);

                // Lưu vào cache
                _cache.MatchDetails[matchId] = payload;

                // Nếu trận đã kết thúc → xóa khỏi live list
                if (payload.Match.IsFinished)
                {
                    _cache.LiveMatchIds.TryRemove(matchId, out _);
                    _logger.LogInformation(
                        "[MatchDetail] Match {Id} FINISHED → removed from live list.", matchId);
                }

                // Push lên hub
                await _hub.Clients
                    .Group($"match-{matchId}")
                    .SendAsync("MatchDetailUpdated", payload, ct);

                _logger.LogDebug(
                    "[MatchDetail] Pushed match {Id}: {H}-{A} ({S})",
                    matchId,
                    payload.Match.Score.FullTime.Home,
                    payload.Match.Score.FullTime.Away,
                    payload.Match.Status);
            }
            catch (RateLimitException ex)
            {
                _logger.LogWarning("[MatchDetail] Rate limit. Retry after {S}s", ex.RetryAfter.TotalSeconds);
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                _logger.LogError(ex, "[MatchDetail] Failed to sync match {Id}.", matchId);
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

            var allMatches = new List<MatchFixtureDto>();
            foreach (var m in arr.EnumerateArray())
            {
                var match = new MatchFixtureDto
                {
                    ExternalId = m.GetProperty("id").GetInt32(),
                    UtcDate = m.GetProperty("utcDate").GetDateTime(),
                    Status = m.GetProperty("status").GetString() ?? "",
                    Matchday = m.TryGetProperty("matchday", out var md)
                                 && md.ValueKind != JsonValueKind.Null ? md.GetInt32() : 0,
                    Stage = m.TryGetProperty("stage", out var st)
                                 && st.ValueKind != JsonValueKind.Null ? st.GetString() ?? "" : "",
                    Group = m.TryGetProperty("group", out var h)
                                 && h.ValueKind != JsonValueKind.Null ? h.GetString() : null,
                    HomeTeam = ParseTeam(m.GetProperty("homeTeam")),
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
                    var byGroup = stageGroup
                        .GroupBy(m => m.Group ?? "")
                        .OrderBy(g => g.Key);

                    foreach (var groupMatches in byGroup)
                    {
                        stageDto.Groups.Add(new GroupDto
                        {
                            GroupName = groupMatches.Key,
                            Matches = groupMatches.OrderBy(m => m.UtcDate).ToList(),
                        });
                    }
                }
                else
                {
                    stageDto.Groups.Add(new GroupDto
                    {
                        GroupName = "",
                        Matches = stageGroup.OrderBy(m => m.UtcDate).ToList(),
                    });
                }

                payload.Stages.Add(stageDto);
            }

            return payload;
        }

        // ── Parse standings → flatten, sort ──────────────────────────────────
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

        // ── Parse match detail → MatchDetailPayload ───────────────────────────
        private static MatchDetailPayload ParseMatchDetail(string json)
        {
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            var score = root.GetProperty("score");
            var ft = score.GetProperty("fullTime");
            var ht = score.GetProperty("halfTime");

            var match = new MatchDetailDto
            {
                ExternalId = root.GetProperty("id").GetInt32(),
                UtcDate = root.GetProperty("utcDate").GetDateTime(),
                Status = root.GetProperty("status").GetString() ?? "",
                Matchday = root.TryGetProperty("matchday", out var md)
                             && md.ValueKind != JsonValueKind.Null ? md.GetInt32() : 0,
                Stage = root.TryGetProperty("stage", out var st)
                             && st.ValueKind != JsonValueKind.Null ? st.GetString() ?? "" : "",
                Group = root.TryGetProperty("group", out var g)
                             && g.ValueKind != JsonValueKind.Null ? g.GetString() : null,
                Venue = root.TryGetProperty("venue", out var v)
                             && v.ValueKind != JsonValueKind.Null ? v.GetString() : null,

                HomeTeam = ParseTeam(root.GetProperty("homeTeam")),
                AwayTeam = ParseTeam(root.GetProperty("awayTeam")),

                Score = new MatchScoreDto
                {
                    Winner = score.TryGetProperty("winner", out var w)
                               && w.ValueKind != JsonValueKind.Null ? w.GetString() : null,
                    Duration = score.TryGetProperty("duration", out var dur)
                               && dur.ValueKind != JsonValueKind.Null
                               ? dur.GetString() ?? "REGULAR" : "REGULAR",
                    FullTime = new ScoreLineDto
                    {
                        Home = GetScoreValue(ft, "home"),
                        Away = GetScoreValue(ft, "away"),
                    },
                    HalfTime = new ScoreLineDto
                    {
                        Home = GetScoreValue(ht, "home"),
                        Away = GetScoreValue(ht, "away"),
                    },
                },

                Referees = ParseReferees(root),
            };

            return new MatchDetailPayload
            {
                UpdatedAt = DateTime.UtcNow,
                Match = match,
            };
        }

        // ── Helpers ───────────────────────────────────────────────────────────
        private static TeamInMatchDto ParseTeam(JsonElement t) => new()
        {
            ExternalId = t.TryGetProperty("id", out var id) && id.ValueKind != JsonValueKind.Null ? id.GetInt32() : 0,
            Name = t.TryGetProperty("name", out var name) && name.ValueKind != JsonValueKind.Null ? name.GetString() ?? "" : "",
            ShortName = t.TryGetProperty("shortName", out var sn) && sn.ValueKind != JsonValueKind.Null ? sn.GetString() ?? "" : "",
            Tla = t.TryGetProperty("tla", out var tla) && tla.ValueKind != JsonValueKind.Null ? tla.GetString() ?? "" : "",
            Crest = t.TryGetProperty("crest", out var cr) && cr.ValueKind != JsonValueKind.Null ? cr.GetString() ?? "" : "",
        };

        private static List<RefereeDto> ParseReferees(JsonElement root)
        {
            var list = new List<RefereeDto>();
            if (!root.TryGetProperty("referees", out var refs)) return list;

            foreach (var r in refs.EnumerateArray())
            {
                list.Add(new RefereeDto
                {
                    Id = r.GetProperty("id").GetInt32(),
                    Name = r.TryGetProperty("name", out var n) && n.ValueKind != JsonValueKind.Null ? n.GetString() ?? "" : "",
                    Nationality = r.TryGetProperty("nationality", out var nat) && nat.ValueKind != JsonValueKind.Null ? nat.GetString() ?? "" : "",
                    Type = r.TryGetProperty("type", out var tp) && tp.ValueKind != JsonValueKind.Null ? tp.GetString() ?? "" : "",
                });
            }

            return list;
        }

        private static int? TryGetScore(JsonElement m, string side)
        {
            if (!m.TryGetProperty("score", out var score)) return null;
            if (!score.TryGetProperty("fullTime", out var ft)) return null;
            if (!ft.TryGetProperty(side, out var val)) return null;
            return val.ValueKind == JsonValueKind.Null ? null : val.GetInt32();
        }

        private static int? GetScoreValue(JsonElement parent, string key)
        {
            if (!parent.TryGetProperty(key, out var val)) return null;
            return val.ValueKind == JsonValueKind.Null ? null : val.GetInt32();
        }
    }
}