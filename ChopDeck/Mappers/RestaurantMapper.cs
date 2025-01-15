using ChopDeck.Dtos.Restaurants;
using ChopDeck.Models;

namespace ChopDeck.Mappers
{
    public static class RestaurantMapper
    {
        public static RestaurantDto ToRestaurantDto(this Restaurant restaurant)
        {
            if (restaurant == null)
            {
                return null;
            }

            return new RestaurantDto

            {
                Id = restaurant.Id,
                UserType = restaurant.ApplicationUser.UserType,
                Name = restaurant.ApplicationUser.Name,
                Email = restaurant.ApplicationUser.Email,
                Address = restaurant.ApplicationUser.Address,
                Lga = restaurant.ApplicationUser.Lga,
                State = restaurant.ApplicationUser.State,
                Description = restaurant.Description,
                CuisineType = restaurant.CuisineType,
                LogoUrl = restaurant.LogoUrl,
                ImageUrl = restaurant.ImageUrl,
                Rating = restaurant.Rating,
                CreatedAt = restaurant.CreatedAt,
                UpdatedAt = restaurant.UpdatedAt,
                IsActive = restaurant.IsActive,
                Status = restaurant.Status,
            };
        }
    }
}