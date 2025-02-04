using System.ComponentModel.DataAnnotations.Schema;
using ChopDeck.Models;

namespace ChopDeck.Models
{
    [Table("Carts")]
    public class Cart
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int? RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        public ICollection<CartItem>? CartItems { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}