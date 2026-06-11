
namespace Football247.Domain.Entities.Stores
{
    public class Cart : BaseEntity
    {
        public Guid UserId { get; set; }

        // Navigation
        public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    }
}
