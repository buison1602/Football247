using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.TeamCmdModel
{
    public class CreateTeamCommandModel
    {
        public string Name { get; set; }
        public string Slug { get; set; }
        public string LogoUrl { get; set; }
    }
}
