using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Restaurants
{
    public class UpdateRestaurantOrderDto
    {
        [Required]
        [RegularExpression("^(AcceptedByRestaurant|DeclinedByRestaurant|OrderPrepared)$",
            ErrorMessage = "Invalid status. Valid values are 'AcceptedByRestaurant', 'DeclinedByRestaurant', or 'OrderPrepared'.")]
        public string? Status { get; set; }
    }
}