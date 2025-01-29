using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ChopDeck.Dtos;
using ChopDeck.Dtos.Restaurants;
using ChopDeck.Helpers;
using ChopDeck.Services.Interfaces;

namespace ChopDeck.Controllers
{
    [ApiController]
    [Route("api/restaurant")]
    public class RestaurantController : ControllerBase
    {


        private readonly IRestaurantService _restaurantService;
        public RestaurantController(IRestaurantService restaurantService)
        {
            _restaurantService = restaurantService;
        }

        /// <summary>
        /// Register restaurant account
        /// </summary>
        /// <param name="createRestaurantDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateRestaurantDto createRestaurantDto)
        {
            var response = await _restaurantService.RegisterAsync(createRestaurantDto);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Restaurant login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _restaurantService.LoginAsync(loginDto);
            return ResponseHelper.HandleResponse(response);
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
            var response = await _restaurantService.GetRestaurantsAsync(queryObject);
            return ResponseHelper.HandleResponse(response);
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
            var response = await _restaurantService.GetRestaurantByIdAsync(id);
            return ResponseHelper.HandleResponse(response);
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
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _restaurantService.UpdateRestaurantAsync(updateDto, userId);
            return ResponseHelper.HandleResponse(response);

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
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _restaurantService.GetOrdersAsync(queryObject, userId);
            return ResponseHelper.HandleResponse(response);
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
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _restaurantService.GetOrderByIdAsync(id, userId);
            return ResponseHelper.HandleResponse(response);
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
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _restaurantService.UpdateOrderStatusAsync(id, updateDto, userId);
            return ResponseHelper.HandleResponse(response);
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
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _restaurantService.DeleteRestaurantAsync(id, userId);
            return ResponseHelper.HandleResponse(response);
        }
    }
}