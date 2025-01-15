using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Drivers
{
    public class UpdateDriverOrderDto
    {
        [Required]
        [RegularExpression("^(DriverAtRestaurant|OrderInTransit|DriverAtAddress|OrderDelivered)$",
            ErrorMessage = "Invalid status. Valid values are 'DriverAtRestaurant', 'OrderInTransit', 'DriverAtAddress' or 'OrderDelivered'.")]
        public string? Status { get; set; }
    }
}