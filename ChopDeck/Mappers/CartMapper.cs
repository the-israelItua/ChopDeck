using ChopDeck.Dtos.Carts;
using ChopDeck.Models;

namespace ChopDeck.Mappers
{
    public static class CartMapper
    {
        public static CartDto? ToCartDto(this Cart cart)
        {
            if (cart == null)
            {
                return null;
            }
            return new CartDto
            {
                Id = cart.Id,
                RestaurantId = cart.RestaurantId,
                CartItems = cart.CartItems?.Select(item => new CartItemDto
                {
                    Id = item.Id,
                    Product = item.Product.ToProductDto(),
                    Quantity = item.Quantity,
                    CartId = item.CartId,
                }).ToList()
            };
        }

        public static CartItemDto ToCartItemDto(this CartItem cartItem)
        {
            return new CartItemDto
            {
                Id = cartItem.Id,
                Product = cartItem.Product.ToProductDto(),
                Quantity = cartItem.Quantity,
            };
        }
    }
}