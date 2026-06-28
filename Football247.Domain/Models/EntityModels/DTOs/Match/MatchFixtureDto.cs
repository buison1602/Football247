using Shared.Enum;

namespace Football247.Domain.Models.EntityModels.DTOs.Match
{
    // ── DTO từng trận ────────────────────────────────────────────────────────
    public class MatchFixtureDto
    {
        public int ExternalId { get; set; }
        public DateTime UtcDate { get; set; }
        public string Status { get; set; } = "";
        public int Matchday { get; set; }
        public string Stage { get; set; } = "";
        public string? Group { get; set; }
        public TeamInMatchDto HomeTeam { get; set; } = new();
        public TeamInMatchDto AwayTeam { get; set; } = new();
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? Winner { get; set; }
        public string DateLabel => UtcDate.ToLocalTime().ToString("dd/MM");
        public string TimeLabel => UtcDate.ToLocalTime().ToString("HH:mm");
        public bool IsLive => Status is "IN_PLAY" or "PAUSED";
        public bool IsFinished => Status == "FINISHED";
    }

    public class TeamInMatchDto
    {
        public int ExternalId { get; set; }
        public string Name { get; set; } = "";
        public string ShortName { get; set; } = "";
        public string Tla { get; set; } = "";
        public string Crest { get; set; } = "";
    }

    // ── Payload danh sách trận (Hub FootballHub) ──────────────────────────────
    public class WcSchedulePayload
    {
        public string CompetitionCode { get; set; } = "WC";
        public DateTime UpdatedAt { get; set; }
        public bool HasLiveMatch { get; set; }
        public List<StageDto> Stages { get; set; } = new();
    }

    public class StageDto
    {
        public string Stage { get; set; } = "";
        public string Label { get; set; } = "";
        public List<GroupDto> Groups { get; set; } = new();
    }

    public class GroupDto
    {
        public string GroupName { get; set; } = "";
        public List<MatchFixtureDto> Matches { get; set; } = new();
    }

    // ── DTO chi tiết 1 trận (Hub MatchDetailHub) ─────────────────────────────
    public class MatchDetailDto
    {
        public int ExternalId { get; set; }
        public DateTime UtcDate { get; set; }
        public string Status { get; set; } = "";
        public int Matchday { get; set; }
        public string Stage { get; set; } = "";
        public string? Group { get; set; }
        public string? Venue { get; set; }          // sân vận động
        public TeamInMatchDto HomeTeam { get; set; } = new();
        public TeamInMatchDto AwayTeam { get; set; } = new();
        public MatchScoreDto Score { get; set; } = new();
        public List<RefereeDto> Referees { get; set; } = new();
        public bool IsLive => Status is "IN_PLAY" or "PAUSED";
        public bool IsFinished => Status == "FINISHED";
    }

    public class MatchScoreDto
    {
        public string? Winner { get; set; }          // HOME_TEAM | AWAY_TEAM | DRAW | null
        public string Duration { get; set; } = "REGULAR"; // REGULAR | EXTRA_TIME | PENALTY_SHOOTOUT
        public ScoreLineDto FullTime { get; set; } = new();
        public ScoreLineDto HalfTime { get; set; } = new();
    }

    public class ScoreLineDto
    {
        public int? Home { get; set; }
        public int? Away { get; set; }
    }

    public class RefereeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Nationality { get; set; } = "";
        public string Type { get; set; } = "";       // REFEREE | ASSISTANT_REFEREE
    }

    // Payload gửi lên MatchDetailHub
    public class MatchDetailPayload
    {
        public DateTime UpdatedAt { get; set; }
        public MatchDetailDto Match { get; set; } = new();
    }
}