namespace Football247.Models.DTOs.Image
{
    public class ImageDto
    {
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; } = 0;
    }
}
