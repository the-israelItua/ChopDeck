using ChopDeck.Models;

namespace ChopDeck.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser applicationUser);
    }
}