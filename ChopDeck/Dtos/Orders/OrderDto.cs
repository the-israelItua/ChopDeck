using ChopDeck.Dtos.Customers;
using ChopDeck.Dtos.Drivers;
using ChopDeck.Dtos.Restaurants;
using ChopDeck.Enums;
using ChopDeck.Models;

namespace ChopDeck.Dtos.Orders
{
    public class OrderDto
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal DeliveryFee { get; set; }
        public int? CustomerId { get; set; }
        public CustomerDto? Customer { get; set; }
        public int? RestaurantId { get; set; }
        public RestaurantDto? Restaurant { get; set; }
        public int? DriverId { get; set; }
        public DriverDto? Driver { get; set; }
        public string Status { get; set; } = OrderStatus.PendingPayment.ToString();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}