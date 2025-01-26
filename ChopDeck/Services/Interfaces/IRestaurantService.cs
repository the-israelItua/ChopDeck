using ChopDeck.Dtos.Restaurants;
using ChopDeck.Dtos.Orders;
using ChopDeck.Dtos;
using ChopDeck.Models;

namespace ChopDeck.Services.Interfaces
{
    public interface IRestaurantService
    {
        Task<ApiResponse<RestaurantDto>> RegisterAsync(CreateRestaurantDto createDto);
        Task<ApiResponse<RestaurantDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<List<RestaurantDto>>> GetRestaurantsAsync(RestaurantQueryObject queryObject);
        Task<ApiResponse<RestaurantDto>> DeleteRestaurantAsync(int id, string userId);
        Task<ApiResponse<RestaurantDto>> GetRestaurantByIdAsync(int id);
        Task<ApiResponse<RestaurantDto>> UpdateRestaurantAsync(UpdateRestaurantDto updateDto, string userId);
        Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int id, UpdateRestaurantOrderDto updateRestaurantOrderDto, string userId);
        Task<ApiResponse<RestaurantOrderDto>> GetOrderByIdAsync(int id, string userId);
        Task<ApiResponse<List<RestaurantOrderListDto>>> GetOrdersAsync(RestaurantOrdersQueryObject queryObj, string userId);
    }
}
