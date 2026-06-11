using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enum
{
    public enum EnumOrderStatus
    {
        Pending = 0,       // Chờ xác nhận
        Confirmed = 1,     // Đã xác nhận
        Processing = 2,    // Đang xử lý / đóng gói
        Shipping = 3,      // Đang giao hàng
        Delivered = 4,     // Đã giao thành công
        Cancelled = 5,     // Đã hủy
        Refunded = 6       // Đã hoàn tiền
    }
}
