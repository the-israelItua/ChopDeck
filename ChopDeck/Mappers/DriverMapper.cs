using ChopDeck.Dtos.Drivers;
using ChopDeck.Models;

namespace ChopDeck.Mappers
{
    public static class DriverMapper
    {
        public static DriverDto? ToDriverDto(this Driver driver)
        {
            if (driver == null)
            {
                return null;
            }
            return new DriverDto
            {
                Id = driver.Id,
                UserType = driver.ApplicationUser.UserType,
                Name = driver.ApplicationUser.Name,
                Email = driver.ApplicationUser.Email,
                Address = driver.ApplicationUser.Address,
                Lga = driver.ApplicationUser.Lga,
                State = driver.ApplicationUser.State,
                VehicleType = driver.VehicleType,
                ProfilePicture = driver.ProfilePicture,
                StateOfOrigin = driver.StateOfOrigin,
                Status = driver.Status
            };
        }
    }
}