using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Restaurants
{
    public class CreateRestaurantDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        [Required]
        public string? CuisineType { get; set; }
        [Required(ErrorMessage = "Logo URL is required.")]
        [Url(ErrorMessage = "Logo URL must be a valid URL.")]
        public string? LogoUrl { get; set; }
        [Required(ErrorMessage = "Image URL is required.")]
        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        public string? ImageUrl { get; set; }
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