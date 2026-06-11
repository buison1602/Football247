namespace Football247.Domain.Models.EntityModels.DTOs.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public Guid CreatedUserId { get; set; }
        public string CreatedFullName { get; set; }
    }
}
