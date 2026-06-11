using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.ArticleCmdModel
{
    public class DeleteArticleCommandModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }
    }
}
