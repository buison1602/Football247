namespace Football247.Domain.Models.EntityModels.DTOs.Role
{
    public class RoleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public List<string> Permissions { get; set; } = new List<string>();
    }
}
