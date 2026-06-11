using Shared.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Football247.Domain.Models.EntityModels.DTOs.Product
{
    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal? SalePrice { get; set; }
        public string? ThumbnailUrl { get; set; }
        public int Stock { get; set; }
        public bool IsActive { get; set; }
        public string? CategoryName { get; set; }
        public DateTime CreatedDate { get; set; }
        public EnumSizeProduct Size { get; set; }

    }
}
