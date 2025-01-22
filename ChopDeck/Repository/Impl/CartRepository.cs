using ChopDeck.Data;
using ChopDeck.Enums;
using ChopDeck.Helpers;
using ChopDeck.Models;
using ChopDeck.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChopDeck.Repository.Impl
{
    public class CartRepository : ICartRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public CartRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public async Task<List<Cart>> GetAsync(string userId)
        {
            return await _applicationDBContext.Carts.Include(c => c.Customer).Where(s => s.Customer.ApplicationUserId == userId).Include(c => c.Restaurant).ToListAsync();
        }

        public async Task<Cart?> GetByIdAsync(int id, string? userId)
        {
            return await _applicationDBContext.Carts
                        .Include(c => c.Restaurant)
                            .ThenInclude(ci => ci.ApplicationUser)
                        .Include(c => c.CartItems)
                            .ThenInclude(ci => ci.Product)
                        .Include(c => c.Customer)
                            .ThenInclude(ci => ci.ApplicationUser)
                        .FirstOrDefaultAsync(s => s.Id == id && s.Customer.ApplicationUserId == userId);
        }
        public async Task<Cart?> GetByRestaurantIdAsync(int? restaurantId, string? userId)
        {
            return await _applicationDBContext.Carts
                        .Include(c => c.Restaurant)
                        .Include(c => c.CartItems)
                        .Include(c => c.Customer)
                        .FirstOrDefaultAsync(s => s.RestaurantId == restaurantId && s.Customer.ApplicationUserId == userId);
        }
        public async Task<Cart> CreateAsync(Cart cart)
        {
            await _applicationDBContext.Carts.AddAsync(cart);
            await _applicationDBContext.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart?> DeleteAsync(int id, string userId)
        {
            var cart = await _applicationDBContext.Carts.Include(c => c.Customer).FirstOrDefaultAsync(s => s.Id == id && s.Customer.ApplicationUserId == userId);
            if (cart == null)
            {
                return null;
            }

            _applicationDBContext.Carts.Remove(cart);
            await _applicationDBContext.SaveChangesAsync();
            return cart;
        }

        public async Task<CartItem> AddItemAsync(CartItem cartItem)
        {
            await _applicationDBContext.CartItems.AddAsync(cartItem);
            await _applicationDBContext.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem> RemoveItemAsync(CartItem cartItem)
        {
            _applicationDBContext.CartItems.Remove(cartItem);
            await _applicationDBContext.SaveChangesAsync();
            return cartItem;
        }

        public async Task<CartItem> UpdateItemAsync(CartItem cartItem)
        {
            _applicationDBContext.CartItems.Update(cartItem);
            await _applicationDBContext.SaveChangesAsync();
            return cartItem;
        }

        public async Task<Order> CheckoutAsync(Cart cart)
        {
            var amount = cart.CartItems.Sum(i => i.Quantity * i.Product.Price);
            var serviceCharge = 300;
            var deliveryFee = 1500;

            var order = new Order
            {
                CustomerId = cart.CustomerId,
                RestaurantId = cart.RestaurantId,
                Amount = amount,
                ServiceCharge = serviceCharge,
                DeliveryFee = deliveryFee,
                TotalAmount = amount + serviceCharge + deliveryFee,
                OrderItems = cart.CartItems.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                }).ToList(),
                Status = OrderStatus.PendingPayment.ToString()
            };

            await _applicationDBContext.Orders.AddAsync(order);
            _applicationDBContext.Carts.Remove(cart);
            await _applicationDBContext.SaveChangesAsync();
            return order;
        }
    }
}