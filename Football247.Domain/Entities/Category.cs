using Football247.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Football247.Models.Entities
{
    public class Category : BaseEntity
    {
        public Category()
        {
            Id = Guid.NewGuid();           
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Slug { get; set; }
    }
}
