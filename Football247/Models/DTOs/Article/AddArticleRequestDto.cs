using System.ComponentModel.DataAnnotations;

namespace Football247.Models.DTOs.Article
{
    public class AddArticleRequestDto
    {
        [Required]
        [MaxLength(150, ErrorMessage = "Title cannot be longer than 150 characters.")]
        public string Title { get; set; }

        [Required]
        [MaxLength(150, ErrorMessage = "Slug cannot be longer than 150 characters.")]
        public string Slug { get; set; }

        [Required]
        [MaxLength(300, ErrorMessage = "Description cannot be longer than 300 characters.")]
        public string Description { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public int Priority { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Must have at least one photo.")]
        public List<string> BgrImg { get; set; }
        
        public byte IsApproved { get; set; } = 0;

        [Required]
        public Guid CreatorId { get; set; }

        [Required]
        public Guid CategoryId { get; set; }

        [MinLength(1, ErrorMessage = "Must have at least one tag.")]
        public List<Guid> TagIds { get; set; } = new List<Guid>();
    }
}
