using System.ComponentModel.DataAnnotations;

namespace ChopDeck.Dtos.Carts
{
    public class CreateCartDto
    {
        [Required]
        public int RestaurantId { get; set; }
    }
}