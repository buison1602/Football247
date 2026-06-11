using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.TagCmdModel
{
    public class UpdateTagCommandModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }

        public string Slug { get; set; }
    }
}
