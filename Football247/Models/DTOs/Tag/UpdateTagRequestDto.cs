using System.ComponentModel.DataAnnotations;

namespace Football247.Models.DTOs.Tag
{
    public class UpdateTagRequestDto
    {
        [Required]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Slug must be at least 3 characters long.")]
        [StringLength(100, ErrorMessage = "Slug cannot be longer than 100 characters.")]
        public string Slug { get; set; }
    }
}
