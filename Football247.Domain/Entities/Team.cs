using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Entities
{
    public class Team : BaseEntity
    {
        public string Name { get; set; }
        public string LogoUrl { get; set; }
        public string Slug { get; set; }
    }
}
