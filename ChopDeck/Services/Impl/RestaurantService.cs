using ChopDeck.Dtos;
using ChopDeck.Dtos.Orders;
using ChopDeck.Dtos.Restaurants;
using ChopDeck.Enums;
using ChopDeck.Mappers;
using ChopDeck.Models;
using ChopDeck.Repository.Interfaces;
using ChopDeck.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Identity;
using Serilog;
using ChopDeck.Helpers;

namespace ChopDeck.Services.Impl
{
    public class RestaurantService : IRestaurantService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRestaurantRepository _restaurantRepo;
        private readonly IOrderRepository _orderRepo;
        private readonly IMemoryCache _cache;
        public RestaurantService(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IRestaurantRepository restaurantRepo, IOrderRepository orderRepo, IMemoryCache cache)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _restaurantRepo = restaurantRepo;
            _orderRepo = orderRepo;
            _cache = cache;
        }
        public async Task<ApiResponse<RestaurantDto>> UpdateRestaurantAsync(UpdateRestaurantDto updateDto, string userId)
        {
            try{
                var restaurant = await _restaurantRepo.GetByUserIdAsync(userId);
                if (restaurant == null)
                {
                    return new ApiResponse<RestaurantDto>
                    {
                        Status = 401,
                        Message = "You do not have permission to perform this operation."
                    };
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Description))
                {
                    restaurant.Description = updateDto.Description;
                }
                if (!string.IsNullOrWhiteSpace(updateDto.CuisineType))
                {
                    restaurant.CuisineType = updateDto.CuisineType;
                }

                if (!string.IsNullOrWhiteSpace(updateDto.LogoUrl))
                {
                    restaurant.LogoUrl = updateDto.LogoUrl;
                }

                if (!string.IsNullOrWhiteSpace(updateDto.ImageUrl))
                {
                    restaurant.ImageUrl = updateDto.ImageUrl;
                }
                if (updateDto.Status == RestaurantStatus.OPEN.ToString() || updateDto.Status == RestaurantStatus.CLOSED.ToString())
                {
                    restaurant.Status = updateDto.Status;
                }


                await _restaurantRepo.UpdateAsync(restaurant);
                return new ApiResponse<RestaurantDto>
                {
                    Status = 200,
                    Message = "Restaurant updated successfully.",
                    Data = restaurant.ToRestaurantDto()
                };
            }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<RestaurantDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<RestaurantDto>> DeleteRestaurantAsync(int id, string userId)
        {
            try{
                var restaurant = await _restaurantRepo.DeleteAsync(id, userId);

                if (restaurant == null)
                {
                    return new ApiResponse<RestaurantDto>
                    {
                        Status = 404,
                        Message = "Restaurant not found."
                    };
                }

                return new ApiResponse<RestaurantDto>
                {
                    Status = 204,
                    Message = "Restaurant deleted successfully"
                };
            }

    
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<RestaurantDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<RestaurantDto>> GetRestaurantByIdAsync(int id)
        {
            try{

                var restaurant = await _restaurantRepo.GetByIdAsync(id);

                if (restaurant == null)
                {
                    return new ApiResponse<RestaurantDto>
                    {
                        Status = 404,
                        Message = "Restaurant not found"
                    };
                }

                return new ApiResponse<RestaurantDto>
                {
                    Status = 200,
                    Message = "Restaurant fetched successfully.",
                    Data = restaurant.ToRestaurantDto()
                };
            }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<RestaurantDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<List<RestaurantDto>>> GetRestaurantsAsync(RestaurantQueryObject queryObject)
        {
            try{
                string cacheKey = $"restaurants_{queryObject.Name}_{queryObject.PageSize}_{queryObject.PageNumber}";

                if (!_cache.TryGetValue(cacheKey, out List<RestaurantDto>? cachedRestaurants))
                {
                    var restaurants = await _restaurantRepo.GetAsync(queryObject);
                    cachedRestaurants = restaurants.Select(r => r.ToRestaurantDto()).ToList();

                    var cacheOptions = CacheHelper.GetCacheOptions();
                    _cache.Set(cacheKey, cachedRestaurants, cacheOptions);
                }

                return new ApiResponse<List<RestaurantDto>>
                {
                    Status = 200,
                    Message = "Restaurant fetched successfully",
                    Data = cachedRestaurants
                };
            }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<List<RestaurantDto>>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<RestaurantDto>> LoginAsync(LoginDto loginDto)
        {
            try
            {
                var restaurant = await _restaurantRepo.GetByEmailAsync(loginDto.Email);
                if (restaurant == null)
                {
                    return new ApiResponse<RestaurantDto>
                    {
                        Status = 401,
                        Message = "Email or password incorrect"
                    };
                }

                var passwordCheck = await _signInManager.CheckPasswordSignInAsync(restaurant.ApplicationUser, loginDto.Password, false);

                if (!passwordCheck.Succeeded)
                {
                    return new ApiResponse<RestaurantDto>
                    {
                        Status = 401,
                        Message = "Email or password incorrect"
                    };
                }

                return new ApiResponse<RestaurantDto>
                {
                    Status = 201,
                    Message = "Login successfully.",
                    Data = restaurant.ToRestaurantDto(),
                    Token = _tokenService.CreateToken(restaurant.ApplicationUser)
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<RestaurantDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<RestaurantDto>> RegisterAsync(CreateRestaurantDto createRestaurantDto)
        {
            try
            {
                var existingRestaurant = await _restaurantRepo.RestaurantEmailExists(createRestaurantDto.Email);
                if (existingRestaurant)
                {
                    return new ApiResponse<RestaurantDto> { Status = 409, Message = "A restaurant with this email already exists." };
                }

                var applicationUser = new ApplicationUser
                {
                    UserName = createRestaurantDto.Email,
                    Email = createRestaurantDto.Email,
                    UserType = "Restaurant",
                    Name = createRestaurantDto.Name,
                    Address = createRestaurantDto.Address,
                    Lga = createRestaurantDto.Lga,
                    State = createRestaurantDto.State,
                };

                var createdUser = await _userManager.CreateAsync(applicationUser, createRestaurantDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(applicationUser, "Restaurant");
                    if (roleResult.Succeeded)
                    {
                        var restaurant = new Restaurant
                        {
                            ApplicationUser = applicationUser,
                            Description = createRestaurantDto.Description,
                            CuisineType = createRestaurantDto.CuisineType,
                            ImageUrl = createRestaurantDto.ImageUrl,
                            LogoUrl = createRestaurantDto.LogoUrl,
                        };

                        await _restaurantRepo.CreateAsync(restaurant);

                        return new ApiResponse<RestaurantDto>
                        {
                            Status = 201,
                            Message = "Restaurant created successfully.",
                            Data = restaurant.ToRestaurantDto(),
                            Token = _tokenService.CreateToken(applicationUser)
                        };

                    }
                    else
                    {
                        var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                        return new ApiResponse<RestaurantDto>
                        {
                            Status = 500,
                            Message = "Failed to assign role",
                        };
                    }
                }
                else
                {
                    var creationErrors = createdUser.Errors
                        .Where(e => !e.Description.Contains("Username", StringComparison.OrdinalIgnoreCase))
                        .Select(e => e.Description)
                        .ToList();
                    return new ApiResponse<RestaurantDto>
                    {
                        Status = 500,
                        Message = "Failed to create user",
                    };
                }
            }
            catch (Exception e)
            {
                return new ApiResponse<RestaurantDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<List<RestaurantOrderListDto>>> GetOrdersAsync(RestaurantOrdersQueryObject queryObject, string userId)
        {
            try
            {
                var orders = await _orderRepo.GetRestaurantOrdersAsync(userId, queryObject);
                return new ApiResponse<List<RestaurantOrderListDto>>
                {
                    Status = 200,
                    Message = "Orders fetched successfully",
                    Data = orders
                };
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<List<RestaurantOrderListDto>>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }
        public async Task<ApiResponse<RestaurantOrderDto>> GetOrderByIdAsync(int id, string userId)
        {
            try{
                var order = await _orderRepo.GetRestaurantOrderByIdAsync(id, userId);

                if (order == null)
                {
                    return new ApiResponse<RestaurantOrderDto>
                    {
                        Status = 404,
                        Message = "Order not found"
                    };
                }

                return new ApiResponse<RestaurantOrderDto>
                {
                    Status = 200,
                    Message = "Order fetched successfully",
                    Data = order
                };
            }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<RestaurantOrderDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int id, UpdateRestaurantOrderDto updateDto, string userId)
        {
            try{

                var order = await _orderRepo.GetOrderByIdAsync(id);
                if (order == null || order.Restaurant.ApplicationUser.Id != userId)

                {
                    return new ApiResponse<OrderDto>
                    {
                        Status = 401,
                        Message = "Order not found"
                    };
                }

                if (!string.IsNullOrWhiteSpace(updateDto.Status))
                {
                    order.Status = updateDto.Status;
                }

                await _orderRepo.UpdateOrderAsync(order);

                return new ApiResponse<OrderDto>
                {
                    Status = 200,
                    Message = "Order status updated successfully.",
                    Data = order.ToOrderDto()
                };

            }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<OrderDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }
    }
}
