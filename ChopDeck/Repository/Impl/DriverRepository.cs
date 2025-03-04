﻿using ChopDeck.Data;
using ChopDeck.Models;
using ChopDeck.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChopDeck.Repository.Impl
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

        public async Task<Driver?> DeleteAsync(Driver driver)
        {
            _applicationDBContext.Drivers.Remove(driver);
            await _applicationDBContext.SaveChangesAsync();
            return driver;
        }
    }
}