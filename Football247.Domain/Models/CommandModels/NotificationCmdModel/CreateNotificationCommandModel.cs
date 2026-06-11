using Shared.Enum.EnumNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.CommandModels.NotificationCmdModel
{
    public class CreateNotificationCommandModel
    {
        /// <summary>
        /// Gửi cho 1 user cụ thể (optional nếu dùng UserIds)
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gửi cho nhiều user (optional)
        /// </summary>
        public List<Guid> UserIds { get; set; } = new List<Guid>();

        public string Title { get; set; } = string.Empty;

        public string Message { get; set; } = string.Empty;

        public string? RedirectUrl { get; set; }

        public Guid? ObjectId { get; set; }

        public EnumNotificationType NotificationType { get; set; } = EnumNotificationType.System;

        /// <summary>
        /// Có push realtime qua Hub không
        /// </summary>
        public bool IsSendRealtime { get; set; } = true;

        /// <summary>
        /// Có gửi email không
        /// </summary>
        public bool IsSendEmail { get; set; } = false;
    }
}
