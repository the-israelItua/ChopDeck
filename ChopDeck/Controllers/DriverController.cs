﻿using System.Security.Claims;
using ChopDeck.Dtos;
using ChopDeck.Dtos.Drivers;
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
    [Route("/api/driver")]
    public class DriverController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDriverRepository _driverRepo;
        private readonly IOrderRepository _orderRepo;

        public DriverController(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IDriverRepository driverRepo, IOrderRepository orderRepo)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _driverRepo = driverRepo;
            _orderRepo = orderRepo;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateDriverDto createDriverDto)
        {
            try
            {
                var existingDriver = await _driverRepo.DriverEmailExists(createDriverDto.Email);
                if (existingDriver)
                {
                    return Conflict(new ErrorResponse<string> { Status = 409, Message = "A Driver with this email already exists." });
                }

                var applicationUser = new ApplicationUser
                {
                    UserName = createDriverDto.Email,
                    Email = createDriverDto.Email,
                    UserType = "Driver",
                    Name = createDriverDto.Name,
                    Address = createDriverDto.Address,
                    Lga = createDriverDto.Lga,
                    State = createDriverDto.State,
                };

                var createdUser = await _userManager.CreateAsync(applicationUser, createDriverDto.Password);
                if (createdUser.Succeeded)
                {
                    var roleResult = await _userManager.AddToRoleAsync(applicationUser, "Driver");
                    if (roleResult.Succeeded)
                    {
                        var Driver = new Driver
                        {
                            ApplicationUser = applicationUser,
                            LicenseNumber = createDriverDto.LicenseNumber,
                            VehicleType = createDriverDto.VehicleType,
                            StateOfOrigin = createDriverDto.StateOfOrigin,
                            ProfilePicture = createDriverDto.ProfilePicture,
                        };

                        await _driverRepo.CreateAsync(Driver);

                        return StatusCode(201, new ApiResponse<DriverDto>
                        {
                            Status = 201,
                            Message = "Driver created successfully.",
                            Data = Driver.ToDriverDto(),
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
            var driver = await _driverRepo.GetByEmailAsync(loginDto.Email);
            if (driver == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                });
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(driver.ApplicationUser, loginDto.Password, false);

            if (!passwordCheck.Succeeded)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                });
            }

            return StatusCode(201, new ApiResponse<DriverDto>
            {
                Status = 201,
                Message = "Login successfully.",
                Data = driver.ToDriverDto(),
                Token = _tokenService.CreateToken(driver.ApplicationUser)
            });
        }
        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetDriverByID([FromRoute] int id)
        {
            var driver = await _driverRepo.GetByIdAsync(id);

            if (driver == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Driver not found"
                });
            }

            return Ok(new ApiResponse<DriverDto>
            {
                Status = 200,
                Message = "Driver fetched successfully.",
                Data = driver.ToDriverDto()
            });
        }

        [HttpPatch]
        [Authorize]
        public async Task<IActionResult> UpdateDriver([FromRoute] int id, UpdateDriverDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var driver = await _driverRepo.GetByUserIdAsync(userId);
            if (driver == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "You do not have permission to perform this operation."
                });
            }

            if (!string.IsNullOrWhiteSpace(updateDto.LicenseNumber))
            {
                driver.LicenseNumber = updateDto.LicenseNumber;
            }
            if (!string.IsNullOrWhiteSpace(updateDto.VehicleType))
            {
                driver.VehicleType = updateDto.VehicleType;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.ProfilePicture))
            {
                driver.ProfilePicture = updateDto.ProfilePicture;
            }

            if (!string.IsNullOrWhiteSpace(updateDto.StateOfOrigin))
            {
                driver.StateOfOrigin = updateDto.StateOfOrigin;
            }
            if (updateDto.Status == DriverStatus.Available.ToString() || updateDto.Status == DriverStatus.Closed.ToString() || updateDto.Status == DriverStatus.OnLeave.ToString())
            {
                driver.Status = updateDto.Status;
            }


            await _driverRepo.UpdateAsync(driver);
            return Ok(new ApiResponse<DriverDto>
            {
                Status = 200,
                Message = "Driver updated successfully.",
                Data = driver.ToDriverDto()
            });

        }

        [HttpDelete]
        [Route("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteDriver([FromRoute] int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var driver = await _driverRepo.DeleteAsync(id, userId);

            if (driver == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Driver not found."
                });
            }

            return NoContent();
        }

        [HttpGet("/orders")]
        [Authorize]
        public async Task<IActionResult> GetAvaliableOrders([FromBody] PaginationQueryObject queryObject)
        {
            {
                var orders = await _orderRepo.GetPreparedOrdersAsync(queryObject);
                return Ok(new ApiResponse<List<RestaurantOrderListDto>>
                {
                    Status = 200,
                    Message = "Orders fetched successfully",
                    Data = orders
                });
            }
        }

        [HttpPatch("/order/assign/{orderId:int}")]
        [Authorize]
        public async Task<IActionResult> AssignOrder([FromRoute] int orderId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;
            var driver = await _driverRepo.GetByEmailAsync(userEmail);
            if (driver == null)
            {
                return Unauthorized(new ErrorResponse<string>
                {
                    Status = 401,
                    Message = "You don't permission to perform this task"
                });
            }

            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return NotFound(new ErrorResponse<string>
                {
                    Status = 404,
                    Message = "Order not found."
                });
            }

            order.DriverId = driver.Id;
            order.Status = OrderStatus.AssignedToDriver.ToString();
            await _orderRepo.UpdateOrderAsync(order);
            return Ok(new ApiResponse<OrderDto>
            {
                Status = 200,
                Message = "Driver assigned to order.",
                Data = order.ToOrderDto()
            });
        }

        [HttpPatch("{id:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateOrderStatus([FromRoute] int id, UpdateDriverOrderDto updateDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var order = await _orderRepo.GetOrderByIdAsync(id);
            if (order == null || order.Driver.ApplicationUser.Id != userId)

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
    }
}