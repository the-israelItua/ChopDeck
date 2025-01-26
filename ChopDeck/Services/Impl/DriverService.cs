using ChopDeck.Dtos;
using ChopDeck.Dtos.Drivers;
using ChopDeck.Dtos.Orders;
using ChopDeck.Dtos.Restaurants;
using ChopDeck.Enums;
using ChopDeck.Models;
using ChopDeck.Repository.Interfaces;
using ChopDeck.Services.Interfaces;
using ChopDeck.Mappers;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace ChopDeck.Services.Impl
{
    public class DriverService : IDriverService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDriverRepository _driverRepo;
        private readonly IOrderRepository _orderRepo;

        public DriverService(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IDriverRepository driverRepo, IOrderRepository orderRepo)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _driverRepo = driverRepo;
            _orderRepo = orderRepo;
        }

        public async Task<ApiResponse<OrderDto>> AssignOrderAsync(int orderId, string userEmail)
        {
            try{
            var driver = await _driverRepo.GetByEmailAsync(userEmail);
            if (driver == null)
            {
                return new ApiResponse<OrderDto>
                {
                    Status = 401,
                    Message = "You don't permission to perform this task"
                };
            }

            var order = await _orderRepo.GetOrderByIdAsync(orderId);

            if (order == null)
            {
                return new ApiResponse<OrderDto>
                {
                    Status = 404,
                    Message = "Order not found."
                };
            }

            order.DriverId = driver.Id;
            order.Status = OrderStatus.AssignedToDriver.ToString();
            await _orderRepo.UpdateOrderAsync(order);
            return new ApiResponse<OrderDto>
            {
                Status = 200,
                Message = "Driver assigned to order.",
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

        public async Task<ApiResponse<DriverDto>> DeleteAsync(string userEmail)
        {
            try{
            var driver = await _driverRepo.GetByEmailAsync(userEmail);

            if (driver == null)
            {
                return new ApiResponse<DriverDto>
                {
                    Status = 404,
                    Message = "Driver not found."
                };
            }

            await _driverRepo.DeleteAsync(driver);

            return new ApiResponse<DriverDto>
                {
                    Status = 204,
                    Message = "Driver account deleted."
                };

                      }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<DriverDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<DriverDto>> GetDetailsAsync(string userEmail)
        {try{
            var driver = await _driverRepo.GetByEmailAsync(userEmail);
            if (driver == null)
            {
                return new ApiResponse<DriverDto>
                {
                    Status = 401,
                    Message = "You don't permission to perform this task"
                };
                }

            return new ApiResponse<DriverDto>
            {
                Status = 200,
                Message = "Driver fetched successfully.",
                Data = driver.ToDriverDto()
            };
                  }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<DriverDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<List<RestaurantOrderListDto>>> GetOrdersAsync(PaginationQueryObject queryObject)
        {
            try{
                var orders = await _orderRepo.GetPreparedOrdersAsync(queryObject);
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
                return  new ApiResponse<List<RestaurantOrderListDto>>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<DriverDto>> LoginAsync(LoginDto loginDto)
        {
            try{
 var driver = await _driverRepo.GetByEmailAsync(loginDto.Email);
            if (driver == null)
            {
                return new ApiResponse<DriverDto>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                };
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(driver.ApplicationUser, loginDto.Password, false);

            if (!passwordCheck.Succeeded)
            {
                return new ApiResponse<DriverDto>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                };
            }

            return new ApiResponse<DriverDto>
            {
                Status = 201,
                Message = "Login successfully.",
                Data = driver.ToDriverDto(),
                Token = _tokenService.CreateToken(driver.ApplicationUser)
            };
            } catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<DriverDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            } 
        }

        public async Task<ApiResponse<DriverDto>> RegisterAsync(CreateDriverDto createDriverDto)
        {
          try
            {
                var existingDriver = await _driverRepo.DriverEmailExists(createDriverDto.Email);
                if (existingDriver)
                {
                    return new ApiResponse<DriverDto>{ Status = 409, Message = "A Driver with this email already exists." };
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

                        return  new ApiResponse<DriverDto>
                        {
                            Status = 201,
                            Message = "Driver created successfully.",
                            Data = Driver.ToDriverDto(),
                            Token = _tokenService.CreateToken(applicationUser)
                        };

                    }
                    else
                    {

                        var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                        Log.Error("An error occurred while assigning roles: {@RoleErrors}", roleErrors);
                        return  new ApiResponse<DriverDto>
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
                     Log.Error("An error occurred while assigning roles: {@CreationErrors}", "An error occured");
                    return  new ApiResponse<DriverDto>
                    {
                        Status = 500,
                        Message = "Failed to create user",
                    };
                }
            }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<DriverDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<DriverDto>> UpdateDetailsAsync(string userId, UpdateDriverDto updateDto)
        {
            try{
           var driver = await _driverRepo.GetByUserIdAsync(userId);
            if (driver == null)
            {
                return new ApiResponse<DriverDto>
                {
                    Status = 401,
                    Message = "You do not have permission to perform this operation."
                };
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
            return new ApiResponse<DriverDto>
            {
                Status = 200,
                Message = "Driver updated successfully.",
                Data = driver.ToDriverDto()
            };
      }
            catch (Exception e)
            {
                 Log.Error(e, "An error occured");
                return  new ApiResponse<DriverDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                };
            }
        }

        public async Task<ApiResponse<OrderDto>> UpdateOrderStatusAsync(int id, UpdateDriverOrderDto updateDto, string userId)
        {
     try{
            var order = await _orderRepo.GetOrderByIdAsync(id);
            if (order == null || order.Driver.ApplicationUser.Id != userId)

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
