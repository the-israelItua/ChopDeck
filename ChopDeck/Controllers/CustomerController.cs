using ChopDeck.Dtos;
using ChopDeck.Dtos.Customers;
using ChopDeck.Helpers;
using ChopDeck.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChopDeck.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;

        public CustomerController(ICustomerService customerService){
            _customerService = customerService;
        }

        /// <summary>
        /// Create customer account
        /// </summary>
        /// <param name="createCustomerDto"></param>
        /// <returns></returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateCustomerDto createCustomerDto)
        {
            var response = await _customerService.RegisterAsync(createCustomerDto);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Customer login
        /// </summary>
        /// <param name="loginDto"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var response = await _customerService.LoginAsync(loginDto);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Fetch customer orders
        /// </summary>
        /// <param name="ordersQueryObject"></param>
        /// <returns></returns>
        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] CustomerOrdersQueryObject ordersQueryObject)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _customerService.GetOrdersAsync(ordersQueryObject, userId);
             return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Fetch customer order by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("orders/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetOrderById([FromRoute] int id)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _customerService.GetOrderByIdAsync(id, userId);
            return ResponseHelper.HandleResponse(response);
        }

        /// <summary>
        /// Delete a customer account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            var userId = UserHelper.GetUserId(HttpContext);
            var response = await _customerService.DeleteCustomerAsync(id, userId);
            return ResponseHelper.HandleResponse(response);
        }

    }
}
