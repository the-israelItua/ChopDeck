using System.ComponentModel.DataAnnotations;
using ChopDeck.Enums;

namespace ChopDeck.Dtos.Drivers
{
    public class UpdateDriverDto
    {

        public string LicenseNumber { get; set; } = string.Empty;
        public string VehicleType { get; set; } = string.Empty;
        public string StateOfOrigin { get; set; } = string.Empty;
        public string ProfilePicture { get; set; } = string.Empty;
        [RegularExpression("^(Available|Closed|OnLeave)$", ErrorMessage = "Invalid status. Valid values are 'Available' or 'Closed' or 'OnLeave.")]
        public string Status { get; set; } = DriverStatus.Available.ToString();
    }
}