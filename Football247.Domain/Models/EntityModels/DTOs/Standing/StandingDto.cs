namespace Football247.Domain.Models.EntityModels.DTOs.Standing
{
    public class StandingDto
    {
        public int Position { get; set; }
        public int TeamExternalId { get; set; }
        public string TeamName { get; set; } = "";
        public string TeamShortName { get; set; } = "";
        public string TeamCrest { get; set; } = "";
        public string GroupName { get; set; } = ""; // GROUP_A, GROUP_B, ... (để frontend biết)

        public int PlayedGames { get; set; }
        public int Won { get; set; }
        public int Draw { get; set; }
        public int Lost { get; set; }
        public int Points { get; set; }
        public int GoalsFor { get; set; }
        public int GoalsAgainst { get; set; }
        public int GoalDifference { get; set; }
    }

    // Payload gửi lên Hub
    public class WcStandingsPayload
    {
        public string CompetitionCode { get; set; } = "WC";
        public DateTime UpdatedAt { get; set; }
        public List<StandingDto> Standings { get; set; } = new();
        // 48 đội, flatten, đã sort sẵn
    }
}