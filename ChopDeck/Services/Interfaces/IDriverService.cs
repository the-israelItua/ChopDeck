using ChopDeck.Dtos.Drivers;
using ChopDeck.Dtos;
using ChopDeck.Dtos.Orders;
using ChopDeck.Models;
using ChopDeck.Dtos.Restaurants;

namespace ChopDeck.Services.Interfaces
{
    public interface IDriverService
    {
        Task<ApiResponse<DriverDto>> RegisterAsync(CreateDriverDto createDto);
        Task<ApiResponse<DriverDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<DriverDto>> GetDetailsAsync(string email);
        Task<ApiResponse<DriverDto>> UpdateDetailsAsync(string userId, UpdateDriverDto updateDto);
        Task<ApiResponse<DriverDto>> DeleteAsync(string userEmail);
        Task<ApiResponse<List<RestaurantOrderListDto>>> GetOrdersAsync(PaginationQueryObject ordersQuery);
        Task<ApiResponse<OrderDto>> AssignOrderAsync(int orderId, string email);
        Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int id, UpdateDriverOrderDto updateDriverOrderDto, string userId);
    }
}
