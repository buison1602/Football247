namespace Football247.Domain.Models.EntityModels.DTOs.Dashboard
{
    public class TopCategoryDto
    {
        public string CategoryName { get; set; } = string.Empty;
        public int TotalViews { get; set; }
        public int ArticleCount { get; set; }
    }
}
