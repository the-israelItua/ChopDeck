using ChopDeck.Data;
using ChopDeck.Dtos.Products;
using ChopDeck.Models;
using ChopDeck.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ChopDeck.Repository.Impl
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        public ProductRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        public async Task<List<Product>> GetProductsAsync(ProductsQueryObject productsQuery)
        {
            var products = _applicationDBContext.Products.AsQueryable().Where(p => p.RestaurantId == productsQuery.RestaurantId);

            if (!string.IsNullOrWhiteSpace(productsQuery.Name))
            {
                products = products.Where(p => p.Name.Contains(productsQuery.Name));
            }

            products = products.OrderBy(p => p.Name);
            var skipNumber = (productsQuery.PageNumber - 1) * productsQuery.PageSize;

            return await products.Skip(skipNumber).Take(productsQuery.PageSize).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _applicationDBContext.Products.Include(c => c.Restaurant).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _applicationDBContext.Products.AddAsync(product);
            await _applicationDBContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _applicationDBContext.Products.Update(product);
            await _applicationDBContext.SaveChangesAsync();
            return product;
        }

        public async Task<Product?> DeleteAsync(int id, string userId)
        {
            var product = await _applicationDBContext.Products.Include(c => c.Restaurant).FirstOrDefaultAsync(s => s.Id == id && s.Restaurant.ApplicationUserId == userId);
            if (product == null)
            {
                return null;
            }

            _applicationDBContext.Products.Remove(product);
            await _applicationDBContext.SaveChangesAsync();
            return product;
        }
    }
}