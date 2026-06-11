using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Standing
{
    public class StandingDto
    {
        public int Position { get; set; }
        public string TeamShortName { get; set; } = string.Empty;
        public string TeamCrest { get; set; } = string.Empty;
        public int PlayedGames { get; set; }
        public int Won { get; set; }
        public int Draw { get; set; }
        public int Lost { get; set; }
        public int GoalDifference { get; set; }
        public int Points { get; set; }
    }
}
