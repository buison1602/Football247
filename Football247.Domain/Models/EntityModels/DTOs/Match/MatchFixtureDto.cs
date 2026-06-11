using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Match
{
    // Dùng cho list fixtures
    public class MatchFixtureDto
    {
        public int ExternalId { get; set; }
        public DateTime UtcDate { get; set; }
        public EnumMatchStatus Status { get; set; }
        public int Matchday { get; set; }

        public TeamInMatchDto HomeTeam { get; set; } = new();
        public TeamInMatchDto AwayTeam { get; set; } = new();

        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }

        // Helper cho frontend groupBy
        public string MatchDateLabel => UtcDate.ToString("dd/MM");
        public string MatchTimeLabel => UtcDate.ToString("HH:mm");
        public bool IsFinished => Status == EnumMatchStatus.Finished;
    }

    public class TeamInMatchDto
    {
        public int ExternalId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ShortName { get; set; } = string.Empty;
        public string Crest { get; set; } = string.Empty;
    }
}
