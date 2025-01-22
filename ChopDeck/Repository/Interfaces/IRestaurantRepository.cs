using ChopDeck.Helpers;
using ChopDeck.Models;

namespace ChopDeck.Repository.Interfaces
{
    public interface IRestaurantRepository
    {
        Task<List<Restaurant>> GetAsync(RestaurantQueryObject queryObject);
        Task<Restaurant?> GetByIdAsync(int id);
        Task<Restaurant?> GetByUserIdAsync(string id);
        Task<Restaurant?> GetByEmailAsync(string email);
        Task<Restaurant> CreateAsync(Restaurant restaurant);
        Task<Restaurant> UpdateAsync(Restaurant restaurant);
        Task<Restaurant?> DeleteAsync(int id, string userId);
        Task<bool> RestaurantEmailExists(string email);
        Task<bool> RestaurantExists(int id);
    }
}