using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Entities
{
    public class Standing : BaseEntity
    {
        public string CompetitionCode { get; set; } = string.Empty; // "PL", "CL"
        public int Season { get; set; }                              // 2025
        public int Position { get; set; }                           // #

        // Team
        public int TeamExternalId { get; set; }
        public string TeamName { get; set; } = string.Empty;        // "Arsenal FC"
        public string TeamShortName { get; set; } = string.Empty;   // "Arsenal"
        public string TeamCrest { get; set; } = string.Empty;       // URL logo

        // Stats — đúng 6 cột trong UI
        public int PlayedGames { get; set; }    // P
        public int Won { get; set; }            // W
        public int Draw { get; set; }           // D
        public int Lost { get; set; }           // L
        public int GoalDifference { get; set; } // GD
        public int Points { get; set; }         // PTS

        public DateTime LastSyncedAt { get; set; }
    }
}
