using Shared.Enum.EnumNotification;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Entities
{
    public class NotificationMessage : BaseEntity
    {
        /// <summary>
        /// Người nhận thông báo
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Người gửi thông báo
        /// </summary>
        public Guid? SenderId { get; set; }


        /// <summary>
        /// Nội dung text là gì?
        /// Ví dụ: "Xác nhận đơn hàng"
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// "Đơn hàng #XYZ của bạn đã được xác nhận thành công."
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 3. Hành động khi bấm vào
        /// Bấm vào UI sẽ link qua đâu? Ví dụ: "/orders/XYZ" hoặc "/articles/123"
        /// </summary>
        public string? RedirectUrl { get; set; }

        /// <summary>
        /// 4. Liên quan đến đối tượng nào?
        /// Dành cho việc tra cứu sau này: ID của Bài viết hoặc ID của Đơn hàng
        /// </summary>
        public Guid? ObjectId { get; set; }

        public EnumNotificationType NotificationType { get; set; }
    }
}
