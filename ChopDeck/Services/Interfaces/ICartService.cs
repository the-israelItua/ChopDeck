using ChopDeck.Dtos.Carts;
using ChopDeck.Dtos.Orders;
using ChopDeck.Models;

namespace ChopDeck.Services.Interfaces
{
    public interface ICartService
    {
        Task<ApiResponse<List<CartDto>>> GetCartsAsync(string userId);
        Task<ApiResponse<CartDto>> GetCartByIdAsync(int id, string userId);
        Task<ApiResponse<string>> DeleteCartAsync(int id, string userId);
        Task<ApiResponse<CartDto>> AddItemAsync(AddCartItemDto addCartItemDto, string userId, string userEmail);
        Task<ApiResponse<CartDto>> RemoveItemAsync(int cartId, int cartItemId, string userId);
        Task<ApiResponse<CartDto>> UpdateItemQuantityAsync(UpdateCartItemQuantityDto quantityDto, int cartId, int cartItemId, string userId);
        Task<ApiResponse<OrderDto>> CheckoutAsync(int cartId, string userId);
    }
}
