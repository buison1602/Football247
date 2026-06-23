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
        public string Stage { get; set; } = ""; // GROUP_STAGE, LAST_32, ...
        public string? Group { get; set; }       // GROUP_A ... GROUP_L (chỉ có ở GROUP_STAGE)

        public TeamInMatchDto HomeTeam { get; set; } = new();
        public TeamInMatchDto AwayTeam { get; set; } = new();

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? Winner { get; set; } // HOME_TEAM | AWAY_TEAM | DRAW | null

        // Helper cho frontend
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
        public string Tla { get; set; } = ""; // MEX, BRA, ...
        public string Crest { get; set; } = "";
    }

    // ── Payload gửi lên Hub ───────────────────────────────────────────────────
    // Cấu trúc: Stages → Groups → Matches
    // GROUP_STAGE: nhiều groups (GROUP_A ... GROUP_L)
    // LAST_32, LAST_16, ...: chỉ 1 group với groupName = tên stage
    public class WcSchedulePayload
    {
        public string CompetitionCode { get; set; } = "WC";
        public DateTime UpdatedAt { get; set; }
        public bool HasLiveMatch { get; set; }
        public List<StageDto> Stages { get; set; } = new();
    }

    public class StageDto
    {
        public string Stage { get; set; } = "";  // GROUP_STAGE | LAST_32 | ...
        public string Label { get; set; } = "";  // "Vòng bảng" | "Vòng 32" | ...
        public List<GroupDto> Groups { get; set; } = new();
    }

    public class GroupDto
    {
        public string GroupName { get; set; } = ""; // GROUP_A | "" (knockout)
        public List<MatchFixtureDto> Matches { get; set; } = new();
    }
}