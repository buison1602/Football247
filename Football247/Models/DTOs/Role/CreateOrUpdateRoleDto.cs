using System.ComponentModel.DataAnnotations;

namespace Football247.Models.DTOs.Role
{
    public class CreateOrUpdateRoleDto
    {
        [Required]
        public string Name { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
