using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enum.EnumNotification
{
    public enum EnumNotificationType
    {
        System = 0,         // Thông báo hệ thống
        NewArticle = 1,     // Bài viết mới
        CommentReply = 2,   // Có người reply comment
        OrderStatus = 3,    // Cập nhật trạng thái đơn hàng
        SpinReward = 4,      // Nhận được lượt quay
        RequestReviewArticle = 5, // Yêu cầu review bài viết
        ReviewRequestArticle = 6, // Review bài viết
    }
}
