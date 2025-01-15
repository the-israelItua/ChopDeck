using ChopDeck.Models;
using ChopDeck.Enums;

namespace ChopDeck.Models
{
    public class Restaurant
    {
        public int Id { get; set; }
        public string? Description { get; set; }
        public string? CuisineType { get; set; }
        public string? LogoUrl { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Rating { get; set; } = 5;
        public string? ApplicationUserId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = RestaurantStatus.CLOSED.ToString();
        public ApplicationUser? ApplicationUser { get; set; }
        public ICollection<Product>? Products { get; set; }
        public ICollection<Order>? Orders { get; set; }
        public ICollection<Cart>? Carts { get; set; }

    }
}