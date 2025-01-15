﻿namespace ChopDeck.Dtos.Products
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int? RestaurantId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}