
using Shared.Enum;

namespace Football247.Domain.Entities.Stores
{
    public class Product : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; } // giá khuyến mãi, nếu có
        public int Stock { get; set; } // số lượng tồn kho 
        public string? ThumbnailUrl { get; set; }
        public string? Images { get; set; }       // JSON array string: ["url1","url2"]
        public bool IsActive { get; set; } = true;
        public Guid ProductCategoryId { get; set; }
        public EnumSizeProduct Size { get; set; } // Size sản phẩm (S, M, L, XL, XXL)

        // Navigation
        public ProductCategory? ProductCategory { get; set; }
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
