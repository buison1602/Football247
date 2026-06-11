using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Enum
{
    public enum EnumPaymentStatus
    {
        Unpaid = 0,        // Chưa thanh toán
        Paid = 1,          // Đã thanh toán
        Refunded = 2,      // Đã hoàn tiền
        Failed = 3         // Thanh toán thất bại
    }
}
