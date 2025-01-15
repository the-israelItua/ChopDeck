using ChopDeck.Data;
using ChopDeck.Interfaces;
using ChopDeck.Models;
using Microsoft.EntityFrameworkCore;

namespace ChopDeck.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public CustomerRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        public async Task<Customer> CreateAsync(Customer customer)
        {
            await _applicationDBContext.Customers.AddAsync(customer);
            await _applicationDBContext.SaveChangesAsync();
            return customer;
        }

        public async Task<Customer?> GetByEmailAsync(string email)
        {
            return await _applicationDBContext.Customers.Include(c => c.ApplicationUser).FirstOrDefaultAsync(s => s.ApplicationUser.Email == email);
        }

        public async Task<bool> CustomerEmailExists(string email)
        {
            return await _applicationDBContext.Customers.Include(c => c.ApplicationUser).AnyAsync(s => s.ApplicationUser.Email == email);
        }

        public async Task<Customer?> DeleteAsync(int id, string userId)
        {
            var customer = await _applicationDBContext.Customers.FirstOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == userId);
            if (customer == null)
            {
                return null;
            }

            _applicationDBContext.Customers.Remove(customer);
            await _applicationDBContext.SaveChangesAsync();
            return customer;
        }
    }
}