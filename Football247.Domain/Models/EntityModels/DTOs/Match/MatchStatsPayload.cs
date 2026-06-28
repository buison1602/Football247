using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Match
{
    public class MatchTeamStatsDto
    {
        public string TeamId { get; set; } = "";
        public string TeamName { get; set; } = "";
        public double Possession { get; set; }       // % kiểm soát bóng
        public int Shots { get; set; }               // tổng cú sút
        public int ShotsOnTarget { get; set; }       // sút trúng đích
        public double Xg { get; set; }               // expected goals
        public int Passes { get; set; }              // số đường chuyền
        public double PassAccuracy { get; set; }     // % chuyền chính xác
    }

    // ── Tổng hợp sự kiện trận đấu ────────────────────────────────────────────
    public class MatchEventsStatsDto
    {
        public int Goals { get; set; }
        public int YellowCards { get; set; }
        public int RedCards { get; set; }
        public int Substitutions { get; set; }
    }

    // ── Full stats payload của 1 trận ────────────────────────────────────────
    public class MatchStatsDto
    {
        public string MatchId { get; set; } = "";        // "m-w26-gj-2v4"
        public string Slug { get; set; } = "";           // "vong-bang-j-jordan-vs-algeria"
        public string Status { get; set; } = "";         // "completed" | "live" | "upcoming"
        public int Minute { get; set; }                  // phút thi đấu hiện tại
        public int HomeScore { get; set; }
        public int AwayScore { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string DataSource { get; set; } = "";     // "fifa_live"
        public string DataSourceLabel { get; set; } = ""; // "FIFA Match Centre / Opta"
        public string? XgEstimateNote { get; set; }

        public MatchTeamStatsDto Home { get; set; } = new();
        public MatchTeamStatsDto Away { get; set; } = new();
        public MatchEventsStatsDto Events { get; set; } = new();

        // Computed helpers cho frontend
        public bool IsLive => Status is "live" or "in_play";
        public bool IsCompleted => Status == "completed";
    }

    // ── Payload gửi qua Hub ───────────────────────────────────────────────────
    public class MatchStatsPayload
    {
        public DateTime PushedAt { get; set; } = DateTime.UtcNow;
        public string Slug { get; set; } = "";       // client dùng slug để route
        public MatchStatsDto Stats { get; set; } = new();
    }
}
