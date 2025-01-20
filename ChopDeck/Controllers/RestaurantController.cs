using System.Security.Claims;
using ChopDeck.Dtos;
using ChopDeck.Dtos.Orders;
using ChopDeck.Dtos.Restaurants;
using ChopDeck.Enums;
using ChopDeck.helpers;
using ChopDeck.Interfaces;
using ChopDeck.Mappers;
using ChopDeck.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChopDeck.Controllers
{
    [ApiController]
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IRestaurantRepository _restaurantRepo;
        private readonly IOrderRepository _orderRepo;

        public RestaurantController(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IRestaurantRepository restaurantRepo, IOrderRepository orderRepo)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _restaurantRepo = restaurantRepo;
            _orderRepo = orderRepo;
        }

        /// <summary>
        /// Register restaurant account
        /// </summary>
        /// <param name="createRestaurantDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateRestaurantDto createRestaurantDto)
        {
            try
            {
                var existingRestaurant = await _restaurantRepo.RestaurantEmailExists(createRestaurantDto.Email);
                if (existingRestaurant)
                {
                    return Conflict(new ErrorResponse<string> { Status = 409, Message = "A restaurant with this email already exists." });
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

                        return StatusCode(201, new ApiResponse<RestaurantDto>
                        {
                            Status = 201,
                            Message = "Restaurant created successfully.",
                            Data = restaurant.ToRestaurantDto(),
                            Token = _tokenService.CreateToken(applicationUser)
                        });

                    }
                    else
                    {
                        var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                        return StatusCode(500, new ErrorResponse<List<string>>
                        {
                            Status = 500,
                            Message = "Failed to assign role",
                            Data = roleErrors
                        });
                    }
                }
                else
                {
                    var creationErrors = createdUser.Errors
                        .Where(e => !e.Description.Contains("Username", StringComparison.OrdinalIgnoreCase))
                        .Select(e => e.Description)
                        .ToList();
                    return StatusCode(500, new ErrorResponse<List<string>>
                    {
                        Status = 500,
                        Message = "Failed to create user",
                        Data = creationErrors
                    });
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, new ErrorResponse<string>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                    Data = e.Message
                });
            }
        }

        /// <summary>
        /// Restaurant login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var restaurant = await _restaurantRepo.GetByEmailAsync(loginDto.Email);
            if (restaurant == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                });
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(restaurant.ApplicationUser, loginDto.Password, false);

            if (!passwordCheck.Succeeded)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                });
            }

            return StatusCode(201, new ApiResponse<RestaurantDto>
            {
                Status = 201,
                Message = "Login successfully.",
                Data = restaurant.ToRestaurantDto(),
                Token = _tokenService.CreateToken(restaurant.ApplicationUser)
            });
        }

        /// <summary>
        /// Fetch restaurants
        /// </summary>
        /// <param name="queryObject"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetRestaurants([FromQuery] RestaurantQueryObject queryObject)
        {
            var restaurants = await _restaurantRepo.GetAsync(queryObject);
            var mappedRestaurants = restaurants.Select(r => r.ToRestaurantDto()).ToList();
            return Ok(new ApiResponse<List<RestaurantDto>>
            {
                Status = 200,
                Message = "Restaurant fetched successfully",
                Data = mappedRestaurants
            });
        }

        /// <summary>
        /// Get restaurant by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetRestaurantByID([FromRoute] int id)
        {
            var restaurant = await _restaurantRepo.GetByIdAsync(id);

            if (restaurant == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Restaurant not found"
                });
            }

            return Ok(new ApiResponse<RestaurantDto>
            {
                Status = 200,
                Message = "Restaurant fetched successfully.",
                Data = restaurant.ToRestaurantDto()
            });
        }

        /// <summary>
        /// Update restaurant details
        /// </summary>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateRestaurant([FromBody] UpdateRestaurantDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var restaurant = await _restaurantRepo.GetByUserIdAsync(userId);
            if (restaurant == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "You do not have permission to perform this operation."
                });
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
            return Ok(new ApiResponse<RestaurantDto>
            {
                Status = 200,
                Message = "Restaurant updated successfully.",
                Data = restaurant.ToRestaurantDto()
            });

        }

        /// <summary>
        /// Get restaurant orders
        /// </summary>
        /// <param name="queryObject"></param>
        /// <returns></returns>
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetRestaurantOrders([FromQuery] RestaurantOrdersQueryObject queryObject)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orders = await _orderRepo.GetRestaurantOrdersAsync(userId, queryObject);
            return Ok(new ApiResponse<List<RestaurantOrderListDto>>
            {
                Status = 200,
                Message = "Orders fetched successfully",
                Data = orders
            });
        }

        /// <summary>
        /// Get order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("orders/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetRestaurantOrderById([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var order = await _orderRepo.GetRestaurantOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Order not found"
                });
            }

            return Ok(new ApiResponse<RestaurantOrderDto>
            {
                Status = 200,
                Message = "Order fetched successfully",
                Data = order
            });
        }

        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPatch("orders/{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, UpdateRestaurantOrderDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = await _orderRepo.GetOrderByIdAsync(id);
            if (order == null || order.Restaurant.ApplicationUser.Id != userId)

            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "Order not found"
                });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.Status))
            {
                order.Status = updateDto.Status;
            }

            await _orderRepo.UpdateOrderAsync(order);

            return Ok(new ApiResponse<OrderDto>
            {
                Status = 200,
                Message = "Order status updated successfully.",
                Data = order.ToOrderDto()
            });

        }

        /// <summary>
        /// Delete restaurant account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteRestaurant([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var restaurant = await _restaurantRepo.DeleteAsync(id, userId);

            if (restaurant == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Restaurant not found."
                });
            }

            return NoContent();
        }

    }
}