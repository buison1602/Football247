using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.ArticleCmdModel
{
    public class UpdateArticleCommandModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public string BgrImage { get; set; }

        public string Content { get; set; }

        public int Priority { get; set; }

        public byte IsApproved { get; set; } = 0;

        public Guid CategoryId { get; set; }

        public List<Guid> TagIds { get; set; } = new List<Guid>();
    }
}
