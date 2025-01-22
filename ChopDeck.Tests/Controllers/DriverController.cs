using ChopDeck.Controllers;
using ChopDeck.Dtos.Drivers;
using ChopDeck.Dtos;
using ChopDeck.Repository.Interfaces;
using ChopDeck.Models;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ChopDeck.Helpers;
using ChopDeck.Dtos.Orders;
using Castle.Core.Resource;
using Microsoft.AspNetCore.Http.HttpResults;
using System.Net;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace ChopDeck_Tests.Controllers
{

    public class DriverControllerTest
    {
        private readonly DriverController _driverController;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IDriverRepository _driverRepo;
        private readonly IOrderRepository _orderRepo;
        public DriverControllerTest()
        {
            _userManager = A.Fake<UserManager<ApplicationUser>>();
            _tokenService = A.Fake<ITokenService>();
            _signInManager = A.Fake<SignInManager<ApplicationUser>>();
            _driverRepo = A.Fake<IDriverRepository>();
            _orderRepo = A.Fake<IOrderRepository>();

            _driverController = new DriverController(_userManager, _tokenService, _signInManager, _driverRepo, _orderRepo);
        }

        [Fact]
        public async Task DriverController_Register_ReturnsDriverExists()
        {
            var createDriverDto = Helpers.CreateDriverDto();
            A.CallTo(() => _driverRepo.DriverEmailExists(createDriverDto.Email)).Returns(true);

            var result = await _driverController.Register(createDriverDto);

            result.Should().BeOfType<ConflictObjectResult>()
             .Which.Value.Should().BeOfType<ErrorResponse<string>>()
             .Which.Status.Should().Be(409);

            result.As<ConflictObjectResult>()
                .Value.As<ErrorResponse<string>>()
                .Message.Should().Be("A Driver with this email already exists.");
        }

        [Fact]
        public async Task DriverController_Register_ReturnsSuccessful()
        {
            var createDriverDto = Helpers.CreateDriverDto();

            A.CallTo(() => _driverRepo.DriverEmailExists(createDriverDto.Email)).Returns(false);

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

            A.CallTo(() => _userManager.CreateAsync(A<ApplicationUser>._, createDriverDto.Password))
                .Returns(Task.FromResult(IdentityResult.Success));

            A.CallTo(() => _userManager.AddToRoleAsync(A<ApplicationUser>._, "Driver"))
        .Returns(Task.FromResult(IdentityResult.Success));

            A.CallTo(() => _tokenService.CreateToken(A<ApplicationUser>._))
                .Returns("mockToken");

            var result = await _driverController.Register(createDriverDto);

            result.Should().BeOfType<ObjectResult>()
                 .Which.StatusCode.Should().Be(201);

            var response = (result as ObjectResult)?.Value as ApiResponse<DriverDto>;
            response.Should().NotBeNull();
            response.Message.Should().Be("Driver created successfully.");
            response.Token.Should().NotBeNullOrEmpty();
        }
        [Fact]
        public async Task DriverController_Register_ReturnsServerErrorOnFailure()
        {
            var createDriverDto = Helpers.CreateDriverDto();

            A.CallTo(() => _driverRepo.DriverEmailExists(createDriverDto.Email)).Returns(false);

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

            A.CallTo(() => _userManager.CreateAsync(A<ApplicationUser>._, createDriverDto.Password))
            .Returns(Task.FromResult(IdentityResult.Failed(new IdentityError { Description = "Weak password" })));

            var result = await _driverController.Register(createDriverDto);

            var objectResult = result.Should().BeOfType<ObjectResult>().Which;
            objectResult.StatusCode.Should().Be(500);

            var errorResponse = objectResult.Value.As<ErrorResponse<List<string>>>();
            errorResponse.Should().NotBeNull();
            errorResponse!.Status.Should().Be(500);
            errorResponse.Message.Should().Be("Failed to create user");
            errorResponse.Data.Should().Contain("Weak password");
        }

        [Fact]
        public async Task DriverController_Login_ReturnsUnauthorizedWhenNotFound()
        {
            var loginDto = Helpers.LoginDto();

            A.CallTo(() => _driverRepo.GetByEmailAsync(loginDto.Email)).Returns(Task.FromResult<Driver?>(null));

            var result = await _driverController.Login(loginDto);

            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Which;
            unauthorizedResult.StatusCode.Should().Be(401);

            var response = unauthorizedResult.Value.As<ErrorResponse<string>>();
            response.Should().NotBeNull();
            response.Message.Should().Be("Email or password incorrect");
        }

        [Fact]
        public async Task DriverController_Login_ReturnsUnauthorizedWhenIncorrectPassword()
        {
            var loginDto = Helpers.LoginDto();
            var driver = Helpers.CreateDriver();

            A.CallTo(() => _driverRepo.GetByEmailAsync(loginDto.Email)).Returns(Task.FromResult<Driver?>(driver));

            A.CallTo(() => _signInManager.CheckPasswordSignInAsync(driver.ApplicationUser, loginDto.Password, false))
                .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Failed));

            var result = await _driverController.Login(loginDto);

            var unauthorizedResult = result.Should().BeOfType<UnauthorizedObjectResult>().Which;
            unauthorizedResult.StatusCode.Should().Be(401);

            var response = unauthorizedResult.Value.As<ErrorResponse<string>>();
            response.Should().NotBeNull();
            response.Message.Should().Be("Email or password incorrect");
        }

        [Fact]
        public async Task DriverController_Login_ReturnsSuccessfull()
        {
            var loginDto = Helpers.LoginDto();
            var driver = Helpers.CreateDriver();
            A.CallTo(() => _driverRepo.GetByEmailAsync(loginDto.Email)).Returns(Task.FromResult<Driver?>(driver));
            A.CallTo(() => _signInManager.CheckPasswordSignInAsync(driver.ApplicationUser, loginDto.Password, false))
            .Returns(Task.FromResult(Microsoft.AspNetCore.Identity.SignInResult.Success));

            A.CallTo(() => _tokenService.CreateToken(driver.ApplicationUser))
            .Returns("generated_token");

            var result = await _driverController.Login(loginDto);

            var objectResult = result.Should().BeOfType<ObjectResult>().Which;
            objectResult.StatusCode.Should().Be(201);

            var response = objectResult.Value.As<ApiResponse<DriverDto>>();
            response.Should().NotBeNull();
            response.Message.Should().Be("Login successfully.");
            response.Token.Should().Be("generated_token");
        }

        [Fact]
        public async Task DriverController_GetDriverById_ReturnsNotFound()
        {
            var userId = "12345";
            Helpers.SetUser(_driverController, userId);
            var id = 1;

            A.CallTo(() => _driverRepo.GetByIdAsync(id)).Returns(Task.FromResult<Driver?>(null));

            var result = await _driverController.GetDriverByID();
            var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Which;
            notFoundResult.StatusCode.Should().Be(404);

            var errorResponse = notFoundResult.Value.As<ErrorResponse<string>>();
            errorResponse.Should().NotBeNull();
            errorResponse.Status.Should().Be(404);
            errorResponse.Message.Should().Be("Driver not found.");
        }

        [Fact]
        public async Task DriverController_GetDriverByID_ReturnsSuccessfully()
        {
            var userId = "1234";
            Helpers.SetUser(_driverController, userId);
            var id = 1;
            var driver = Helpers.CreateDriver();
            A.CallTo(() => _driverRepo.GetByIdAsync(id)).Returns(driver);
            var result = await _driverController.GetDriverByID();

            var okResult = result.Should().BeOfType<OkObjectResult>().Which;
            okResult.StatusCode.Should().Be(200);

            var apiResponse = okResult.Value.As<ApiResponse<DriverDto>>();
            apiResponse.Should().NotBeNull();
            apiResponse.Status.Should().Be(200);
            apiResponse.Message.Should().Be("Driver fetched successfully.");
            apiResponse.Data.Should().BeOfType<DriverDto>();
        }


        // [Fact]
        // public async Task DriverController_GetOrders_ReturnSuccessfully()
        // {
        //     var ordersQueryObject = new DriverOrdersQueryObject();
        //     var userId = "12345";
        //     Helpers.SetUser(_driverController, userId);

        //     var orders = new List<Order>
        //         {
        //             new Order { Id = 1,DriverId = 1, Driver = CreateDriver(), Restaurant = CreateRestaurant(), Driver = CreateDriver(), TotalAmount = 100, Amount= 100, DeliveryFee = 20,ServiceCharge = 10 },
        //             new Order { Id = 2, Driver = CreateDriver(),  Restaurant = CreateRestaurant(), Driver = CreateDriver(), TotalAmount = 100, Amount= 100, DeliveryFee = 20,ServiceCharge = 10 },
        //      };


        //     A.CallTo(() => _orderRepo.GetDriverOrdersAsync(userId, ordersQueryObject)).Returns(orders);
        //     var result = await _driverController.GetOrders(ordersQueryObject);
        //     var okResult = result.Should().BeOfType<OkObjectResult>().Which;
        //     okResult.StatusCode.Should().Be(200);

        //     var apiResponse = okResult.Value.As<ApiResponse<List<OrderDto>>>();
        //     apiResponse.Should().NotBeNull();
        //     apiResponse.Status.Should().Be(200);
        //     apiResponse.Message.Should().Be("Orders fetched successfully");
        //     apiResponse.Data.Should().HaveCount(2);
        //     apiResponse.Data[0].Should().BeOfType<OrderDto>();
        // }

        // [Fact]
        // public async Task DriverController_GetOrderById_ReturnsNotFound()
        // {
        //     var userId = "1234";
        //     Helpers.SetUser(_driverController, userId);
        //     var id = 1;
        //     A.CallTo(() => _orderRepo.GetDriverOrderByIdAsync(id, userId)).Returns(Task.FromResult<Order?>(null));
        //     var result = await _driverController.GetDriverOrderById(id);
        //     var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Which;
        //     notFoundResult.StatusCode.Should().Be(404);

        //     var errorResponse = notFoundResult.Value.As<ErrorResponse<string>>();
        //     errorResponse.Should().NotBeNull();
        //     errorResponse.Status.Should().Be(404);
        //     errorResponse.Message.Should().Be("Order not found");
        // }

        // [Fact]
        // public async Task DriverController_GetOrderById_ReturnsSuccessfully()
        // {
        //     var userId = "12334";
        //     Helpers.SetUser(_driverController, userId);
        //     var id = 1;
        //     var order = new Order { Id = 1, DriverId = 1, Driver = CreateDriver(), Restaurant = CreateRestaurant(), Driver = CreateDriver(), TotalAmount = 100, Amount = 100, DeliveryFee = 20, ServiceCharge = 10 };
        //     A.CallTo(() => _orderRepo.GetDriverOrderByIdAsync(id, userId)).Returns(order);
        //     var result = await _driverController.GetDriverOrderById(id);

        //     var okResult = result.Should().BeOfType<OkObjectResult>().Which;
        //     okResult.StatusCode.Should().Be(200);

        //     var apiResponse = okResult.Value.As<ApiResponse<OrderDto>>();
        //     apiResponse.Should().NotBeNull();
        //     apiResponse.Status.Should().Be(200);
        //     apiResponse.Message.Should().Be("Order fetched successfully");
        //     apiResponse.Data.Should().NotBeNull();
        // }

        // [Fact]
        // public async Task DriverController_DeleteDriver_ReturnsNotFound()
        // {
        //     var userId = "12334";
        //     Helpers.SetUser(_driverController, userId);
        //     var id = 1;
        //     A.CallTo(() => _driverRepo.DeleteAsync(id, userId)).Returns(Task.FromResult<Driver?>(null));
        //     var result = await _driverController.DeleteDriver(id);

        //     var notFoundResult = result.Should().BeOfType<NotFoundObjectResult>().Which;
        //     notFoundResult.StatusCode.Should().Be(404);

        //     var errorResponse = notFoundResult.Value.As<ErrorResponse<string>>();
        //     errorResponse.Should().NotBeNull();
        //     errorResponse.Status.Should().Be(404);
        //     errorResponse.Message.Should().Be("Driver not found.");
        // }

        // [Fact]
        // public async Task DriverController_DeleteDriver_ReturnsSuccessfully()
        // {
        //     var userId = "12334";
        //     Helpers.SetUser(_driverController, userId);
        //     var id = 1;
        //     var driver = Helpers.CreateDriver();
        //     A.CallTo(() => _driverRepo.DeleteAsync(id, userId)).Returns(Driver);
        //     var result = await _driverController.DeleteDriver(id);

        //     result.Should().BeOfType<NoContentResult>();
        // }
    }
}