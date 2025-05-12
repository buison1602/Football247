using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class Category
    {
        public Category()
        {
            Id = Guid.NewGuid();           
            CreatedAt = DateTime.UtcNow;   
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
