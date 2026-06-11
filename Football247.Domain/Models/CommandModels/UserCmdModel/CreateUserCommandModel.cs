using System.ComponentModel.DataAnnotations;

namespace Football247.Domain.Models.CommandModels.UserCmdModel
{
    public class CreateUserCommandModel
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }


        public string? Role { get; set; }
    }
}
