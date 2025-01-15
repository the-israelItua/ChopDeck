using ChopDeck.Enums;

namespace ChopDeck.Dtos.Restaurants
{
    public class RestaurantDto
    {
        public int Id { get; set; }
        public string UserType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Lga { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CuisineType { get; set; }
        public string? LogoUrl { get; set; }
        public string? ImageUrl { get; set; }
        public decimal? Rating { get; set; } = 5;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public bool IsActive { get; set; } = true;
        public string Status { get; set; } = RestaurantStatus.CLOSED.ToString();
    }
}