using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Order
{
    // Response - Danh sách đơn hàng (gọn)
    public class OrderListDto
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public EnumOrderStatuss Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public EnumPaymentMethodd PaymentMethod { get; set; }
        public EnumPaymentStatus PaymentStatus { get; set; }
        public string PaymentStatusName { get; set; } = string.Empty;
        public int TotalItems { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
