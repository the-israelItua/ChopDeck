using System.ComponentModel.DataAnnotations.Schema;
using ChopDeck.Models;

namespace ChopDeck.Models
{
    [Table("CartItems")]
    public class CartItem
    {
        public int Id { get; set; }
        public int? CartId { get; set; }
        public Cart? Cart { get; set; }
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int Quantity { get; set; }
    }
}