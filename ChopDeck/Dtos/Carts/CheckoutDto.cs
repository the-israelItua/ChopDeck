using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Carts
{
    public class CheckoutDto
    {
        [Required]
        public int CartId { get; set; }
    }
}