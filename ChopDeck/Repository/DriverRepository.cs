using ChopDeck.Data;
using ChopDeck.Interfaces;
using ChopDeck.Models;
using Microsoft.EntityFrameworkCore;

namespace ChopDeck.Repository
{
    public class DriverRepository : IDriverRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public DriverRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public async Task<Driver?> GetByIdAsync(int id)
        {
            return await _applicationDBContext.Drivers.Include(r => r.ApplicationUser).FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<Driver?> GetByUserIdAsync(string id)
        {
            return await _applicationDBContext.Drivers.Include(r => r.ApplicationUser).FirstOrDefaultAsync(s => s.ApplicationUserId == id);
        }
        public async Task<Driver?> GetByEmailAsync(string email)
        {
            return await _applicationDBContext.Drivers.Include(r => r.ApplicationUser).FirstOrDefaultAsync(s => s.ApplicationUser.Email == email);
        }
        public async Task<Driver> CreateAsync(Driver driver)
        {
            await _applicationDBContext.Drivers.AddAsync(driver);
            await _applicationDBContext.SaveChangesAsync();
            return driver;
        }
        public async Task<Driver> UpdateAsync(Driver driver)
        {
            _applicationDBContext.Drivers.Update(driver);
            await _applicationDBContext.SaveChangesAsync();
            return driver;
        }
        public async Task<bool> DriverEmailExists(string email)
        {
            return await _applicationDBContext.Drivers.Include(c => c.ApplicationUser).AnyAsync(s => s.ApplicationUser.Email == email);
        }
        public async Task<bool> DriverExists(int id)
        {
            return await _applicationDBContext.Drivers.AnyAsync(s => s.Id == id);
        }

        public async Task<Driver?> DeleteAsync(int id, string userId)
        {
            var driver = await _applicationDBContext.Drivers.FirstOrDefaultAsync(s => s.Id == id && s.ApplicationUserId == userId);
            if (driver == null)
            {
                return null;
            }

            _applicationDBContext.Drivers.Remove(driver);
            await _applicationDBContext.SaveChangesAsync();
            return driver;
        }
    }
}