using System.ComponentModel.DataAnnotations;

namespace Football247.Domain.Models.CommandModels.AuthCommand
{
    public class LoginCommandModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
