using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.TeamCmdModel
{
    public class UpdateTeamCommandModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public string LogoUrl { get; set; }
    }
}
