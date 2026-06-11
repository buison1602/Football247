using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Football247.Domain.Models.CommandModels.ArticleCmdModel
{
    public class CreateArticleCommandModel
    {
        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public string BgrImage { get; set; }

        public string Content { get; set; }

        public int Priority { get; set; }

        [JsonIgnore]
        public byte IsApproved { get; set; } = 1;

        public Guid CategoryId { get; set; }

        public Guid? TeamId { get; set; }

        public List<Guid> TagIds { get; set; } = new List<Guid>();
    }
}
