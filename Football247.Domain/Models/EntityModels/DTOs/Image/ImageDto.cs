namespace Football247.Domain.Models.EntityModels.DTOs.Image
{
    public class ImageDto
    {
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
}
