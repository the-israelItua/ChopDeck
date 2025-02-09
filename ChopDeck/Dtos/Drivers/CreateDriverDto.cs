using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Drivers
{
    public class CreateDriverDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public string LicenseNumber { get; set; } = string.Empty;
        [Required]
        public string VehicleType { get; set; } = string.Empty;
        [Required]
        public string StateOfOrigin { get; set; } = string.Empty;
        [Required(ErrorMessage = "Profile picture is required.")]
        [Url(ErrorMessage = "Profile picture must be a valid URL.")]
        public string ProfilePicture { get; set; } = string.Empty;
        [Required]
        public string Address { get; set; } = string.Empty;
        [Required]
        public string Lga { get; set; } = string.Empty;
        [Required]
        public string State { get; set; } = string.Empty;
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        [MinLength(8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).+$",
        ErrorMessage = "Password must have at least one uppercase letter, one number, and one special character.")]
        public string Password { get; set; } = string.Empty;
    }
}