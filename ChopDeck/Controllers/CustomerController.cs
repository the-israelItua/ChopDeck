using System.Security.Claims;
using ChopDeck.Models;
using ChopDeck.Dtos;
using ChopDeck.Dtos.Customers;
using ChopDeck.Dtos.Orders;
using ChopDeck.helpers;
using ChopDeck.Interfaces;
using ChopDeck.Mappers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChopDeck.Controllers
{
    [ApiController]
    [Route("api/customer")]
    public class CustomerController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;

        public CustomerController(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, ICustomerRepository customerRepo, IOrderRepository orderRepo)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateCustomerDto createCustomerDto)
        {
            try
            {
                var existingCustomer = await _customerRepo.CustomerEmailExists(createCustomerDto.Email);
                if (existingCustomer)
                {
                    return Conflict(new ErrorResponse<string> { Status = 409, Message = "A customer with this email already exists." });
                }

                var applicationUser = new ApplicationUser
                {
                    UserName = createCustomerDto.Email,
                    Email = createCustomerDto.Email,
                    UserType = "Customer",
                    Name = createCustomerDto.Name,
                    Address = createCustomerDto.Address,
                    Lga = createCustomerDto.Lga,
                    State = createCustomerDto.State,
                };

                var createdUser = await _userManager.CreateAsync(applicationUser, createCustomerDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(applicationUser, "CUSTOMER");
                    if (roleResult.Succeeded)
                    {
                        var customer = new Customer
                        {
                            ApplicationUser = applicationUser,
                        };

                        await _customerRepo.CreateAsync(customer);

                        return StatusCode(201, new ApiResponse<CustomerDto>
                        {
                            Status = 201,
                            Message = "Customer created successfully.",
                            Data = customer.ToCustomerDto(),
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var customer = await _customerRepo.GetByEmailAsync(loginDto.Email);
            if (customer == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                });
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(customer.ApplicationUser, loginDto.Password, false);

            if (!passwordCheck.Succeeded)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                });
            }

            return StatusCode(201, new ApiResponse<CustomerDto>
            {
                Status = 201,
                Message = "Login successfully.",
                Data = customer.ToCustomerDto(),
                Token = _tokenService.CreateToken(customer.ApplicationUser)
            });
        }

        [HttpGet("orders")]
        [Authorize]
        public async Task<IActionResult> GetOrders([FromQuery] CustomerOrdersQueryObject ordersQueryObject)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orders = await _orderRepo.GetCustomerOrdersAsync(userId, ordersQueryObject);
            var mappedOrders = orders.Select(s => s.ToOrderDto()).ToList();
            return Ok(new ApiResponse<List<OrderDto>>
            {
                Status = 200,
                Message = "Orders fetched successfully",
                Data = mappedOrders
            });
        }

        [HttpGet("orders/{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetCstomerOrderById([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var order = await _orderRepo.GetCustomerOrderByIdAsync(id, userId);

            if (order == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Order not found"
                });
            }

            return Ok(new ApiResponse<OrderDto>
            {
                Status = 200,
                Message = "Order fetched successfully",
                Data = order.ToOrderDto()
            });
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteCustomer([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var customer = await _customerRepo.DeleteAsync(id, userId);

            if (customer == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Customer not found."
                });
            }

            return NoContent();
        }

    }
}
