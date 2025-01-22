using ChopDeck.Models;

namespace ChopDeck.Repository.Interfaces
{
    public interface IDriverRepository
    {
        Task<Driver?> GetByIdAsync(int id);
        Task<Driver?> GetByUserIdAsync(string id);
        Task<Driver?> GetByEmailAsync(string email);
        Task<Driver> CreateAsync(Driver driver);
        Task<Driver> UpdateAsync(Driver driver);
        Task<Driver?> DeleteAsync(Driver driver);
        Task<bool> DriverEmailExists(string email);
        Task<bool> DriverExists(int id);
    }
}