
namespace Football247.Domain.Entities.Stores
{
    // Bảng trung gian để giải quyết quan hệ Nhiều - Nhiều (N - N) giữa Cart và Product.
    public class CartItem : BaseEntity
    {
        public Guid CartId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTime { get; set; }   // Giá tại thời điểm thêm vào giỏ

        // Navigation
        public Cart? Cart { get; set; }
        public Product? Product { get; set; }
    }
}
