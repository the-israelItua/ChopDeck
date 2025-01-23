using ChopDeck.Dtos.Carts;
using ChopDeck.Repository.Interfaces;
using ChopDeck.Models;
using ChopDeck.Services.Interfaces;
using ChopDeck.Mappers;
using ChopDeck.Dtos.Orders;
using Serilog;


namespace ChopDeck.Services.Impl{
    public class CustomerService: ICustomerService{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;

        public CustomerService(UserManager<ApplicationUser> userManager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, ICustomerRepository customerRepo, IOrderRepository orderRepo)
        {
            _userManager = userManager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _customerRepo = customerRepo;
            _orderRepo = orderRepo;
        }

        public async Task<ApiResponse<CustomerDto>> RegisterAsync(CreateCustomerDto createDto){
            try
            {
                var existingCustomer = await _customerRepo.CustomerEmailExists(createCustomerDto.Email);
                if (existingCustomer)
                {
                    return new ApiResponse<CustomerDto>{ Status = 409, Message = "A customer with this email already exists." });
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

                        return new ApiResponse<CustomerDto>
                        {
                            Status = 201,
                            Message = "Customer created successfully.",
                            Data = customer.ToCustomerDto(),
                            Token = _tokenService.CreateToken(applicationUser)
                        };

                    }
                    else
                    {
                        var roleErrors = roleResult.Errors.Select(e => e.Description).ToList();
                        Log.Error(roleErrors, "An error occured");
                        return new ApiResponse<CustomerDto>
                        {
                            Status = 500,
                            Message = "Failed to assign role",
                            Data = roleErrors
                        };
                    }
                }
                else
                {
                    var creationErrors = createdUser.Errors
                        .Where(e => !e.Description.Contains("Username", StringComparison.OrdinalIgnoreCase))
                        .Select(e => e.Description)
                        .ToList();
                     Log.Error(creationErrors, "An error occured");
                    return new ApiResponse<CustomerDto>
                    {
                        Status = 500,
                        Message = "Failed to create user",
                        Data = creationErrors
                    };
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<CustomerDto>
                {
                    Status = 500,
                    Message = "An unexpected error occurred",
                    Data = e.Message
                };
            }
        }

        public async Task<ApiResponse<CustomerDto>> LoginAsync(LoginDto loginDto){
          try{
            var customer = await _customerRepo.GetByEmailAsync(loginDto.Email);
            if (customer == null)
            {
                return new <ApiResponse<CustomerDto>>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                };
            }

            var passwordCheck = await _signInManager.CheckPasswordSignInAsync(customer.ApplicationUser, loginDto.Password, false);

            if (!passwordCheck.Succeeded)
            {
             return new <ApiResponse<CustomerDto>>
                {
                    Status = 401,
                    Message = "Email or password incorrect"
                };
            }

            return new <ApiResponse<CustomerDto>>
            {
                Status = 201,
                Message = "Login successfully.",
                Data = customer.ToCustomerDto(),
                Token = _tokenService.CreateToken(customer.ApplicationUser)
            };
            } catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<CustomerDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<List<OrderDto>>> GetOrdersAsync(CustomerOrdersQueryObject ordersQuery, string userId){
           try{
             var orders = await _orderRepo.GetCustomerOrdersAsync(userId, ordersQueryObject);
            var mappedOrders = orders.Select(s => s.ToOrderDto()).ToList();
            return new ApiResponse<List<OrderDto>>
            {
                Status = 200,
                Message = "Orders fetched successfully",
                Data = mappedOrders
            };
           }catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new ApiResponse<CustomerDto>
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<OrderDto>> GetOrderByIdAsync(int id, string userId){
           try{
            var order = await _orderRepo.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return <ApiResponse<OrderDto>> 
                {
                    Status = 404,
                    Message = "Order not found"
                };
            }

            return <ApiResponse<OrderDto>> 
            {
                Status = 200,
                Message = "Order fetched successfully",
                Data = order.ToOrderDto()
            };
           }catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new <ApiResponse<OrderDto>> 
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<OrderDto>> DeleteCustomerAsync(int id, string userId){
           try{
            var order = await _orderRepo.GetOrderByIdAsync(id, userId);

            if (order == null)
            {
                return <ApiResponse<OrderDto>> 
                {
                    Status = 404,
                    Message = "Order not found"
                };
            }

            return <ApiResponse<OrderDto>> 
            {
                Status = 200,
                Message = "Order fetched successfully",
                Data = order.ToOrderDto()
            };
           }catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new <ApiResponse<OrderDto>> 
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }

        public async Task<ApiResponse<string>> DeleteCustomerAsync(int id, string userId){
            try{
                var customer = await _customerRepo.DeleteAsync(id, userId);

                if (customer == null)
                {
                    return new <ApiResponse<string>>
                    {
                        Status = 404,
                        Message = "Customer not found."
                    };
                }

                 return new ApiResponse<string>
                {
                    Status = 204,
                    Message = 'Customer account deleted.'
                };
                
            }catch (Exception e)
            {
                Log.Error(e, "An error occured");
                return new <ApiResponse<OrderDto>> 
                {
                    Status = 500,
                    Message = "An error occurred while processing your request.",
                };
            }
        }
    }
}