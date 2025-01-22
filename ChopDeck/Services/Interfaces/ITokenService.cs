using ChopDeck.Models;

namespace ChopDeck.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser applicationUser);
    }
}