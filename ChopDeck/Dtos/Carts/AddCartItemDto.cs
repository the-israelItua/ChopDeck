using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Carts
{
    public class AddCartItemDto
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
        public int Quantity { get; set; }
    }
}