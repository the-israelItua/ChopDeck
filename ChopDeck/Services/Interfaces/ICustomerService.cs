using ChopDeck.Dtos.Carts;
using ChopDeck.Dtos.Orders;
using ChopDeck.Models;

namespace ChopDeck.Services.Interfaces
{
    public interface ICartService
    {
        Task<ApiResponse<CustomerDto>> RegisterAsync(CreateCustomerDto createDto);
        Task<ApiResponse<CustomerDto>> LoginAsync(LoginDto loginDto);
        Task<ApiResponse<List<OrderDto>>> GetOrdersAsync(CustomerOrdersQueryObject ordersQuery, string userId);
        Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id, string userId);
        Task<ApiResponse<string> DeleteCustomerAsync(int id, string userId);
    }
}
