using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Carts
{
    public class UpdateCartItemQuantityDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }
    }
}