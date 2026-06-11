using Shared.Enum;

namespace Football247.Domain.Entities.Stores
{
    public class Order : BaseEntity
    {
        public string OrderCode { get; set; } = string.Empty;   // VD: ORD-20250517-0001
        public Guid UserId { get; set; }
        public decimal TotalAmount { get; set; }
        public EnumOrderStatuss Status { get; set; } = EnumOrderStatuss.Pending;

        // Thông tin giao hàng
        public string ReceiverName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string ShippingAddress { get; set; } = string.Empty;
        public string? Note { get; set; }

        // Thanh toán
        public EnumPaymentMethodd PaymentMethod { get; set; } = EnumPaymentMethodd.COD;
        public EnumPaymentStatus PaymentStatus { get; set; } = EnumPaymentStatus.Unpaid;

        // Navigation
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
