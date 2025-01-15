using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Products
{
    public class UpdateProductDto
    {
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string? Name { get; set; }
        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        public string? ImageUrl { get; set; }
        [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0 and realistic.")]
        public decimal? Price { get; set; }
    }
}