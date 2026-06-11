using Shared.Enum.EnumNotification;

namespace Football247.Domain.Models.EntityModels.DTOs.Notification
{
    public class NotificationMessageDto
    {
        public Guid? UserId { get; set; }

        public string? Title { get; set; }

        public string? Message { get; set; }

        public string? RedirectUrl { get; set; }

        public Guid? ObjectId { get; set; }

        public EnumNotificationType NotificationType { get; set; }

        public DateTime? CreatedDate { get; set; }
    }
}
