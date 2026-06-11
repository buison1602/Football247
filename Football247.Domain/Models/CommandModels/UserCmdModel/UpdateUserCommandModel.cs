using System.Text.Json.Serialization;

namespace Football247.Domain.Models.CommandModels.UserCmdModel
{
    public class UpdateUserCommandModel
    {
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? AvatarUrl { get; set; }
        public bool ReceiveInAppNotifications { get; set; }
        public bool ReceiveEmailNotifications { get; set; }
    }
}
