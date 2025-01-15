using ChopDeck.Models;
using ChopDeck.Enums;

namespace ChopDeck.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal Amount { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal DeliveryFee { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int? RestaurantId { get; set; }
        public Restaurant? Restaurant { get; set; }
        public int? DriverId { get; set; }
        public Driver? Driver { get; set; }
        public string Status { get; set; } = OrderStatus.PendingPayment.ToString();
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public ICollection<OrderItem>? OrderItems { get; set; }
    }
}