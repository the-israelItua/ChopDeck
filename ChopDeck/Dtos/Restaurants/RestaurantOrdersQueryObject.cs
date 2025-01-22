using System.ComponentModel.DataAnnotations;
using ChopDeck.Enums;

namespace ChopDeck.Helpers
{
    public class RestaurantOrdersQueryObject : PaginationQueryObject
    {
        [RegularExpression("^(PendingRestaurantConfirmation|AcceptedByRestaurant,|DeclinedByRestaurant,|OrderPrepared,|AssignedToDriver,|DriverAtRestaurant,|OrderInTransit,|DriverAtAddress,|OrderDelivered)$", ErrorMessage = "Invalid status. Valid values are PendingRestaurantConfirmation|AcceptedByRestaurant,|DeclinedByRestaurant,|OrderPrepared,|AssignedToDriver,|DriverAtRestaurant,|OrderInTransit,|DriverAtAddress,|OrderDelivered.")]
        public string? Status { get; set; } = OrderStatus.PendingRestaurantConfirmation.ToString();
    }
}