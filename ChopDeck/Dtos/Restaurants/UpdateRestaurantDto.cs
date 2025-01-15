using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Restaurants
{
    public class UpdateRestaurantDto
    {
        public string? Description { get; set; }
        public string? CuisineType { get; set; }
        public string? LogoUrl { get; set; }
        public string? ImageUrl { get; set; }
        [RegularExpression("^(OPEN|CLOSED)$", ErrorMessage = "Invalid status. Valid values are 'OPEN' or 'CLOSED'.")]
        public string? Status { get; set; }
    }
}