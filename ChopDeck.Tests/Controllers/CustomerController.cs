using ChopDeck.Controllers;
using ChopDeck.Dtos.Customers;
using ChopDeck.Dtos;
using ChopDeck.Interfaces;
using ChopDeck.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChopDeck.helpers;
using ChopDeck.Dtos.Orders;

namespace ChopDeck.Tests.Controllers
{

    public class CustomerControllerTest
    {
        private readonly CustomerController _customerController;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ICustomerRepository _customerRepo;
        private readonly IOrderRepository _orderRepo;
        public CustomerControllerTest()
        {
            _userManager = A.Fake<UserManager<ApplicationUser>>();
            _tokenService = A.Fake<ITokenService>();
            _signInManager = A.Fake<SignInManager<ApplicationUser>>();
            _customerRepo = A.Fake<ICustomerRepository>();
            _orderRepo = A.Fake<IOrderRepository>();

            _customerController = new CustomerController(_userManager, _tokenService, _signInManager, _customerRepo, _orderRepo);
        }

        private CreateCustomerDto CreateCustomerDto()
        {
            return new CreateCustomerDto
            {
                Name = "Test User",
                Address = "123 Test St",
                Lga = "Test LGA",
                State = "Test State",
                Email = "test@example.com",
                PhoneNumber = "09160275869",
                Password = "TestPassword123"
            };
        }

        private LoginDto LoginDto()
        {
            return new LoginDto
            {
                Email = "test@example.com",
                Password = "TestPassword123"
            };
        }

        private Customer CreateCustomer()
        {
            var applicationUser = new ApplicationUser
            {
                UserName = "test@gmail.com",
                Email = "test@gmail.com",
                UserType = "Customer",
                Name = "Test",
                Address = "Test address",
                Lga = "Test lga",
                State = "Test state",
            };
            return new Customer
            {
                ApplicationUser = applicationUser,
            };
        }

        [Fact]
        public async Task CustomerController_Register_ReturnsCustomerExists()
        {
            var createCustomerDto = CreateCustomerDto();
            A.CallTo(() => _customerRepo.CustomerEmailExists(createCustomerDto.Email)).Returns(true);

            var result = await _customerController.Register(createCustomerDto);

            result.Should().BeOfType<ConflictObjectResult>()
             .Which.Value.Should().BeOfType<ErrorResponse<string>>()
             .Which.Status.Should().Be(409);

            result.As<ConflictObjectResult>()
                .Value.As<ErrorResponse<string>>()
                .Message.Should().Be("A customer with this email already exists.");
        }

        [Fact]
        public async Task CustomerController_Register_ReturnsSuccessful()
        {
            var createCustomerDto = CreateCustomerDto();

            A.CallTo(() => _customerRepo.CustomerEmailExists(createCustomerDto.Email)).Returns(false);

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

            A.CallTo(() => _userManager.CreateAsync(A<ApplicationUser>._, createCustomerDto.Password))
                .Returns(Task.FromResult(IdentityResult.Success));

            A.CallTo(() => _userManager.AddToRoleAsync(A<ApplicationUser>._, "CUSTOMER"))
        .Returns(Task.FromResult(IdentityResult.Success));

            A.CallTo(() => _tokenService.CreateToken(A<ApplicationUser>._))
                .Returns("mockToken");

            var result = await _customerController.Register(createCustomerDto);

            result.Should().BeOfType<ObjectResult>()
                 .Which.StatusCode.Should().Be(201);

            var response = (result as ObjectResult)?.Value as ApiResponse<CustomerDto>;
            response.Should().NotBeNull();
            response.Message.Should().Be("Customer created successfully.");
            response.Token.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public async Task CustomerController_Register_ReturnsServerErrorOnFailure()
        {
            var createCustomerDto = CreateCustomerDto();

            A.CallTo(() => _customerRepo.CustomerEmailExists(createCustomerDto.Email)).Returns(false);

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

            A.CallTo(() => _userManager.CreateAsync(A<ApplicationUser>._, createCustomerDto.Password))
            .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Weak password" })));

            var result = await _customerController.Register(createCustomerDto);

            var objectResult = result.Should().BeOfType<ObjectResult>().Which;
            objectResult.StatusCode.Should().Be(500);

            var errorResponse = objectResult.Value.As<ErrorResponse<List<string>>>();
            errorResponse.Should().NotBeNull();
            errorResponse!.Status.Should().Be(500);
            errorResponse.Message.Should().Be("Failed to create user");
            errorResponse.Data.Should().Contain("Weak password");
        }

        [Fact]
        public async Task CustomerController_Login_ReturnsUnauthorizedWhenNotFound()
        {
            var loginDto = LoginDto();

            A.CallTo(() => _customerRepo.GetByEmailAsync(loginDto.Email)).Returns(Task.FromResult<Customer?>(null));

            var result = await _customerController.Login(loginDto);

            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Which;
            unauthorizedResult.StatusCode.Should().Be(401);

            var response = unauthorizedResult.Value.As<ErrorResponse<string>>();
            response.Should().NotBeNull();
            response.Message.Should().Be("Email or password incorrect");
        }

        [Fact]
        public async Task CustomerController_Login_ReturnsUnauthorizedWhenIncorrectPassword()
        {
            var loginDto = LoginDto();
            var customer = CreateCustomer();

            A.CallTo(() => _customerRepo.GetByEmailAsync(loginDto.Email)).Returns(Task.FromResult<Customer?>(customer));

            A.CallTo(() => _signInManager.CheckPasswordSignInAsync(customer.ApplicationUser, loginDto.Password, false))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed));

            var result = await _customerController.Login(loginDto);

            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Which;
            unauthorizedResult.StatusCode.Should().Be(401);

            var response = unauthorizedResult.Value.As<ErrorResponse<string>>();
            response.Should().NotBeNull();
            response.Message.Should().Be("Email or password incorrect");
        }

        [Fact]
        public async Task CustomerController_Login_ReturnsSuccessfull()
        {
            var loginDto = LoginDto();
            var customer = CreateCustomer();
            A.CallTo(() => _customerRepo.GetByEmailAsync(loginDto.Email)).Returns(Task.FromResult<Customer?>(customer));
            A.CallTo(() => _signInManager.CheckPasswordSignInAsync(customer.ApplicationUser, loginDto.Password, false))
            .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

            A.CallTo(() => _tokenService.CreateToken(customer.ApplicationUser))
            .Returns("generated_token");

            var result = await _customerController.Login(loginDto);

            var objectResult = result.Should().BeOfType<ObjectResult>().Which;
            objectResult.StatusCode.Should().Be(201);

            var response = objectResult.Value.As<ApiResponse<CustomerDto>>();
            response.Should().NotBeNull();
            response.Message.Should().Be("Login successfully.");
            response.Token.Should().Be("generated_token");
        }

        [Fact]
        public async Task CustomerController_GetOrders_ReturnSuccessfully()
        {
            var ordersQueryObject = new CustomerOrdersQueryObject();
            var userId = "12345";
            var orders = new List<Order>
                {
                    new Order { Id = 1, TotalAmount = 100, Amount= 100, DeliveryFee = 20,ServiceCharge = 10 },
                    new Order { Id = 1, TotalAmount = 100, Amount= 100, DeliveryFee = 20,ServiceCharge = 10 },
             };
            A.CallTo(() => _orderRepo.GetCustomerOrdersAsync(userId, ordersQueryObject)).Returns(orders);
            var result = await _customerController.GetOrders(ordersQueryObject);
            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            okResult.StatusCode.Should().Be(200);

            var apiResponse = okResult.Value.As<ApiResponse<List<OrderDto>>>();
            apiResponse.Should().NotBeNull();
            apiResponse.Status.Should().Be(200);
            apiResponse.Message.Should().Be("Orders fetched successfully");
            apiResponse.Data.Should().HaveCount(2);
            apiResponse.Data[0].Should().BeOfType<OrderDto>();
        }

    }
}