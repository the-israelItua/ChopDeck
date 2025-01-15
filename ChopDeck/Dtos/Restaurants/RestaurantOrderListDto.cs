﻿using ChopDeck.Dtos.Orders;

namespace ChopDeck.Dtos.Restaurants
{
    public class RestaurantOrderListDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public List<OrderItemDto> OrderItems { get; set; } = new();
    }
}