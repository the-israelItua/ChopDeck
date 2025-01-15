using ChopDeck.Enums;

namespace ChopDeck.Dtos.Drivers
{
    public class DriverDto
    {
        public int Id { get; set; }
        public string UserType { get; set; } = "Driver";
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Lga { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public int RestaurantId { get; set; }
        public string LicenseNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string StateOfOrigin { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        public decimal? Rating { get; set; } = 5;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = DriverStatus.Available.ToString();
    }
}