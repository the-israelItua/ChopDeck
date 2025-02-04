namespace ChopDeck.Dtos.Carts
{
    public class CartDto
    {
        public int Id { get; set; }
        public int? RestaurantId { get; set; }
        public ICollection<CartItemDto>? CartItems { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}