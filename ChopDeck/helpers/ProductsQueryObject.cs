using System.ComponentModel.DataAnnotations;

namespace ChopDeck.helpers
{
    public class ProductsQueryObject : PaginationQueryObject
    {
        public string? Name { get; set; } = string.Empty;
        [Required]
        public int RestaurantId { get; set; }
    }
}