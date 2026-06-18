namespace Football247.Domain.Models.EntityModels.DTOs.Dashboard
{
    public class TopProductDto
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
