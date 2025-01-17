using ChopDeck.Dtos;
using ChopDeck.Dtos.Customers;
using ChopDeck.Dtos.Drivers;
using ChopDeck.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

public static class Helpers
{
    public static void SetUser(ControllerBase controller, string userId)
    {
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }, "TestAuthentication"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };
    }

    public static CreateCustomerDto CreateCustomerDto()
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

    public static CreateDriverDto CreateDriverDto()
    {
        return new CreateDriverDto
        {
            Name = "Test User",
            Address = "123 Test St",
            Lga = "Test LGA",
            State = "Test State",
            Email = "test@example.com",
            PhoneNumber = "09160275869",
            Password = "TestPassword123",
            StateOfOrigin = "Lagos",
            VehicleType = "Motorcycle",
            LicenseNumber = "12345"
        };
    }

    public static LoginDto LoginDto()
    {
        return new LoginDto
        {
            Email = "test@example.com",
            Password = "TestPassword123"
        };
    }

    public static ApplicationUser CreateApplicationUser()
    {
        return new ApplicationUser
        {
            UserName = "test@gmail.com",
            Email = "test@gmail.com",
            UserType = "Customer",
            Name = "Test",
            Address = "Test address",
            Lga = "Test lga",
            State = "Test state",
        };
    }

    public static Customer CreateCustomer()
    {
        return new Customer
        {
            ApplicationUser = CreateApplicationUser(),
        };
    }

    public static Restaurant CreateRestaurant()
    {
        return new Restaurant
        {
            ApplicationUser = CreateApplicationUser(),
        };
    }

    public static Driver CreateDriver()
    {
        return new Driver
        {
            ApplicationUser = CreateApplicationUser(),
        };
    }
}
