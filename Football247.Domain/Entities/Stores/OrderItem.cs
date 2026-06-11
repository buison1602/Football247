namespace Football247.Domain.Entities.Stores
{
    public class OrderItem : BaseEntity
    {
        public Guid OrderId { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;    // Snapshot tên tại thời điểm đặt
        public string? ProductThumbnail { get; set; }              // Snapshot ảnh
        public int Quantity { get; set; }
        public decimal Price { get; set; }                         // Snapshot giá tại thời điểm đặt
        public decimal TotalPrice => Price * Quantity;             // Computed

        // Navigation
        public Order? Order { get; set; }
        public Product? Product { get; set; }
    }
}
