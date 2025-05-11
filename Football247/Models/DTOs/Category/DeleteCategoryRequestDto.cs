using System.ComponentModel.DataAnnotations;

namespace Football247.Models.DTOs.Category
{
    public class DeleteCategoryRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Slug must be at least 3 characters long.")]
        [StringLength(100, ErrorMessage = "Slug cannot be longer than 100 characters.")]
        public string Slug { get; set; }
    }
}
