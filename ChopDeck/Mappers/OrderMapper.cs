using ChopDeck.Dtos.Orders;
using ChopDeck.Models;

namespace ChopDeck.Mappers
{
    public static class OrderMapper
    {
        public static OrderDto? ToOrderDto(this Order order)
        {
            if (order == null)
            {
                return null;
            }
            return new OrderDto
            {
                Id = order.Id,
                TotalAmount = order.TotalAmount,
                Amount = order.Amount,
                ServiceCharge = order.ServiceCharge,
                DeliveryFee = order.DeliveryFee,
                CustomerId = order.CustomerId,
                Customer = order.Customer.ToCustomerDto(),
                RestaurantId = order.RestaurantId,
                Restaurant = order.Restaurant.ToRestaurantDto(),
                DriverId = order.DriverId,
                Driver = order.Driver.ToDriverDto(),
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
        }
    }
}