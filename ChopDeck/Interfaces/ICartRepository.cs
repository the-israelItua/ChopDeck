using ChopDeck.Models;

namespace ChopDeck.helpers
{
    public interface ICartRepository
    {
        public Task<List<Cart>> GetAsync(string userId);
        public Task<Cart> CreateAsync(Cart cart);
        public Task<Cart?> GetByIdAsync(int id, string? userId);
        public Task<Cart?> GetByRestaurantIdAsync(int? id, string? userId);
        public Task<Cart?> DeleteAsync(int id, string userId);
        public Task<CartItem> AddItemAsync(CartItem cartItem);
        public Task<CartItem> RemoveItemAsync(CartItem cartItem);
        public Task<CartItem> UpdateItemAsync(CartItem cartItem);
        public Task<Order> CheckoutAsync(Cart cart);

    }
}