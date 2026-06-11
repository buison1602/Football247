using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Order
{
    public class OrderSummaryDto
    {
        public Guid Id { get; set; }
        public string OrderCode { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public int ItemCount { get; set; }
        public string? ReceiverName { get; set; }
        public string? PhoneNumber { get; set; }
    }
}
