using ChopDeck.Data;
using ChopDeck.Helpers;
using ChopDeck.Models;
using ChopDeck.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChopDeck.Repository.Impl
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public RestaurantRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public async Task<List<Restaurant>> GetAsync(RestaurantQueryObject queryObject)
        {
            var restaurants = _applicationDBContext.Restaurants.Include(r => r.ApplicationUser).AsQueryable();

            if (!string.IsNullOrWhiteSpace(queryObject.Name))
            {
                restaurants = restaurants.Where(r => r.ApplicationUser.Name.Contains(queryObject.Name));
            };

            restaurants = restaurants.OrderBy(p => p.ApplicationUser.Name);
            var skipNumber = (queryObject.PageNumber - 1) * queryObject.PageSize;

            return await restaurants.Skip(skipNumber).Take(queryObject.PageSize).ToListAsync();

        }
        public async Task<Restaurant?> GetByIdAsync(int id)
        {
            return await _applicationDBContext.Restaurants.Include(r => r.ApplicationUser).FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Restaurant?> GetByUserIdAsync(string id)
        {
            return await _applicationDBContext.Restaurants.Include(r => r.ApplicationUser).FirstOrDefaultAsync(s => s.ApplicationUserId == id);
        }
        public async Task<Restaurant?> GetByEmailAsync(string email)
        {
            return await _applicationDBContext.Restaurants.Include(r => r.ApplicationUser).FirstOrDefaultAsync(s => s.ApplicationUser.Email == email);
        }
        public async Task<Restaurant> CreateAsync(Restaurant restaurant)
        {
            await _applicationDBContext.Restaurants.AddAsync(restaurant);
            await _applicationDBContext.SaveChangesAsync();
            return restaurant;
        }
        public async Task<Restaurant> UpdateAsync(Restaurant restaurant)
        {
            _applicationDBContext.Restaurants.Update(restaurant);
            await _applicationDBContext.SaveChangesAsync();
            return restaurant;
        }
        public async Task<bool> RestaurantEmailExists(string email)
        {
            return await _applicationDBContext.Restaurants.Include(c => c.ApplicationUser).AnyAsync(s => s.ApplicationUser.Email == email);
        }
        public async Task<bool> RestaurantExists(int id)
        {
            return await _applicationDBContext.Restaurants.AnyAsync(s => s.Id == id);
        }

        public async Task<Restaurant?> DeleteAsync(int id, string userId)
        {
            var restaurant = await _applicationDBContext.Restaurants.FirstOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == userId);
            if (restaurant == null)
            {
                return null;
            }

            _applicationDBContext.Restaurants.Remove(restaurant);
            await _applicationDBContext.SaveChangesAsync();
            return restaurant;
        }
    }
}