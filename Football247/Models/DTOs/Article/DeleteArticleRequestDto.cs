using System.ComponentModel.DataAnnotations;

namespace Football247.Models.DTOs.Article
{
    public class DeleteArticleRequestDto
    {
        [Required]
        [MaxLength(150, ErrorMessage = "Slug cannot be longer than 150 characters.")]
        public string Slug { get; set; }
    }
}
