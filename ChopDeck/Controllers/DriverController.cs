using System.Security.Claims;
using ChopDeck.Dtos;
using ChopDeck.Dtos.Drivers;
using ChopDeck.Dtos.Orders;
using ChopDeck.Dtos.Restaurants;
using ChopDeck.Enums;
using ChopDeck.Helpers;
using ChopDeck.Repository.Interfaces;
using ChopDeck.Services.Interfaces;
using ChopDeck.Mappers;
using ChopDeck.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;


namespace ChopDeck.Controllers
{
    [ApiController]
    [Route("/api/driver")]
    public class DriverController : ControllerBase
    {
  
        private readonly IDriverService _driverService;
        public DriverController(IDriverService driverService)
        {
            _driverService = driverService;
        }
        /// <summary>
        /// Create driver account
        /// </summary>
        /// <param name="createDriverDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateDriverDto createDriverDto)
        {
            var response = await _driverService.RegisterAsync(createDriverDto);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Driver login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _driverService.LoginAsync(loginDto);
            return ResponseHelper.HandleResponse(response);
        }

       /// <summary>
       /// Get user details
       /// </summary>
       /// <returns></returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetDriver()
        {
            var userEmail = UserHelper.GetUserEmail(HttpContext);
            var response = await _driverService.GetDetailsAsync(userEmail);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Update driver details
        /// </summary>
        /// <param name="updateDto"></param>
        /// <returns></returns>

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateDriver([FromBody] UpdateDriverDto updateDto)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _driverService.UpdateDetailsAsync(userId, updateDto);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Delete driver account
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Authorize]
        public async Task<IActionResult> DeleteDriver()
        {
            var userEmail = UserHelper.GetUserEmail(HttpContext);
          var response = await _driverService.DeleteAsync(userEmail);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Fetch available orders
        /// </summary>
        /// <param name="queryObject"></param>
        /// <returns></returns>
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetAvaliableOrders([FromBody] PaginationQueryObject queryObject)
        {
             var response = await _driverService.GetOrdersAsync(queryObject);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Assign order to driver
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPatch("order/assign/{orderId:int}")]
        [Authorize]
        public async Task<IActionResult> AssignOrder([FromRoute] int orderId)
        {
            var userEmail = UserHelper.GetUserEmail(HttpContext);
             var response = await _driverService.AssignOrderAsync(orderId, userEmail);
            return ResponseHelper.HandleResponse(response);
        }


        /// <summary>
        /// Update order status
        /// </summary>
        /// <param name="id"></param>
        /// <param name="updateDto"></param>
        /// <returns></returns>
        [HttpPatch("order/{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, UpdateDriverOrderDto updateDto)
        {
   var userId = UserHelper.GetUserId(HttpContext);
   var response = await _driverService.UpdateOrderStatusAsync(id, updateDto, userId);
            return ResponseHelper.HandleResponse(response);
        }
    }
}