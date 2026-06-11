using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Cart
{
    public class CartItemDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ProductThumbnail { get; set; }
        public decimal Price { get; set; }             // Giá hiện tại của sản phẩm
        public decimal? SalePrice { get; set; }
        public decimal PriceAtTime { get; set; }       // Giá lúc thêm vào giỏ
        public int Quantity { get; set; }
        public decimal SubTotal { get; set; }          // PriceAtTime * Quantity
        public int StockAvailable { get; set; }        // Tồn kho hiện tại để FE check
    }
}
