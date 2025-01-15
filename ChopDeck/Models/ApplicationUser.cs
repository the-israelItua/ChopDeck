using Microsoft.AspNetCore.Identity;

namespace ChopDeck.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string UserType { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Lga { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}