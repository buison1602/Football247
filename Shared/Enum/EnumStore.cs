using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enum
{
    public enum EnumOrderStatuss
    {
        Pending = 1,        // Chờ xác nhận
        Confirmed = 2,      // Đã xác nhận
        Shipping = 3,       // Đang giao
        Delivered = 4,      // Đã giao
        Cancelled = 5       // Đã hủy
    }

    public enum EnumPaymentMethodd
    {
        COD = 1,            // Thanh toán khi nhận hàng
        Online = 2          // Thanh toán trực tuyến (ví điện tử, thẻ tín dụng, v.v.)
    }

    public enum EnumPaymentStatuss
    {
        Unpaid = 1,         // Chưa thanh toán
        Paid = 2,           // Đã thanh toán
        Refunded = 3        // Đã hoàn tiền
    }
}
