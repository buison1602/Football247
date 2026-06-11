using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Entities
{
    public class Match : BaseEntity
    {
        // ID từ football-data.org (để tránh duplicate khi sync)
        public int ExternalId { get; set; }

        // Thông tin cơ bản
        public DateTime UtcDate { get; set; }
        public EnumMatchStatus Status { get; set; } // TIMED, FINISHED, IN_PLAY, ...
        public int Matchday { get; set; }

        // Home team
        public int HomeTeamExternalId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public string HomeTeamShortName { get; set; } = string.Empty;
        public string HomeTeamCrest { get; set; } = string.Empty;
        public int? HomeScore { get; set; } // null nếu chưa đấu

        // Away team
        public int AwayTeamExternalId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public string AwayTeamShortName { get; set; } = string.Empty;
        public string AwayTeamCrest { get; set; } = string.Empty;
        public int? AwayScore { get; set; } // null nếu chưa đấu

        // Competition
        public string CompetitionCode { get; set; } = string.Empty; // "PL", "CL", ...
        public string CompetitionName { get; set; } = string.Empty;

        // Season
        public int Season { get; set; } // 2025
    }
}
