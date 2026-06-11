using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Order
{
    // Response - Chi tiết đơn hàng (đầy đủ)
    public class OrderDetailDto : OrderSummaryDto
    {
        public string ReceiverName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Note { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }
}
