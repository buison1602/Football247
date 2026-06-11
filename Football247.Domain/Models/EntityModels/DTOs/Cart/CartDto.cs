using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Cart
{
    // Response - Toàn bộ giỏ hàng
    public class CartDto
    {
        public Guid Id { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public int TotalItems { get; set; }            // Tổng số lượng sản phẩm
        public decimal TotalAmount { get; set; }       // Tổng tiền
    }
}
