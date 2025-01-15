using ChopDeck.Dtos.Restaurants;
using ChopDeck.helpers;
using ChopDeck.Models;

namespace ChopDeck.Interfaces
{
    public interface IOrderRepository
    {

        Task<List<RestaurantOrderListDto>> GetRestaurantOrdersAsync(string userId, RestaurantOrdersQueryObject queryObject);
        Task<List<RestaurantOrderListDto>> GetPreparedOrdersAsync(PaginationQueryObject queryObject);
        Task<RestaurantOrderDto?> GetRestaurantOrderByIdAsync(int id, string userId);
        Task<List<Order>> GetCustomerOrdersAsync(string userId, CustomerOrdersQueryObject queryObject);
        Task<Order?> GetCustomerOrderByIdAsync(int id, string userId);
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> UpdateOrderAsync(Order order);
    }
}