using ChopDeck.Models;

namespace ChopDeck.Interfaces
{
    public interface ICustomerRepository
    {
        Task<Customer> CreateAsync(Customer customer);
        Task<Customer?> GetByEmailAsync(string email);
        Task<Customer?> DeleteAsync(int id, string userId);
        Task<bool> CustomerEmailExists(string email);
    }
}