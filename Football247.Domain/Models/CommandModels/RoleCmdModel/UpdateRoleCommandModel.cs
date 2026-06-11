using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.Role
{
    public class UpdateRoleCommandModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
