using System.ComponentModel.DataAnnotations;

namespace Football247.Models.DTOs.User
{
    public class CreateUserDto
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string? Role { get; set; }
    }
}
