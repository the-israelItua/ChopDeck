using ChopDeck.Dtos.Products;

namespace ChopDeck.Dtos.Carts
{
    public class CartItemDto
    {
        public int Id { get; set; }
        public int? CartId { get; set; }
        public ProductDto? Product { get; set; }
        public int Quantity { get; set; }
    }
}