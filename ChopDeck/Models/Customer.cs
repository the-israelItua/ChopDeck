namespace ChopDeck.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string? ApplicationUserId { get; set; } = string.Empty;
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Cart>? Carts { get; set; }
    }
}